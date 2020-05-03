Imports System.ComponentModel


Public Class CrossLinkingPainter

    Public Property DrawingScale As Double

    Private Property UIElementsDictionary() As Dictionary(Of CrosslinkingSourceObjectViewModel, UIElement)
    Private Property UICrosslinkerDictionary() As Dictionary(Of CrossLinkingVisualizer.CrosslinkViewModel, UIElement)
    Private Property CrosslinkUICollection() As List(Of UIElement)

    Public Property ActiveSourceObject As CrosslinkingSourceObjectViewModel
        Get
            Return Parent.Data.SourceObjects.First()
        End Get
        Set(value As CrosslinkingSourceObjectViewModel)

        End Set
    End Property

    Public ReadOnly Property Parent() As VisualizerViewModel
    Public Property DrawWindow As CrossLinkingVisualizationWindow

    Public ReadOnly Property ValueFilterViewModels As List(Of ValueFilterViewModel)
    Public Property TemplateViewModels As List(Of CrosslinkingSourceObjectViewModel)
    Public ReadOnly Property ValueFilters As List(Of IValueFilter)
    Public Property VisibilityFiltersActive As Boolean

    Private HighlightThickeningFactor As Double = 4
    Private HighlightElements As List(Of UIElement)
    Private HighlightedPath As Shape
    Private HighlightLocked As Boolean

    Public Sub New(Parent As VisualizerViewModel)

        Me.Parent = Parent

        UIElementsDictionary = New Dictionary(Of CrosslinkingSourceObjectViewModel, UIElement)
        UICrosslinkerDictionary = New Dictionary(Of CrossLinkingVisualizer.CrosslinkViewModel, UIElement)
        CrosslinkUICollection = New List(Of UIElement)
        ValueFilterViewModels = New List(Of ValueFilterViewModel)
        ValueFilters = New List(Of IValueFilter)

        AddHandler Parent.PropertyChanged, AddressOf FDRHardLimitChangeListener

        DrawWindow = New CrossLinkingVisualizationWindow()

        AddHandler DrawWindow.DrawingCanvas.MouseWheel, AddressOf OnMouseWheel

        With DrawWindow

            .DataContext = Parent
            .DrawingCanvas.Background = New SolidColorBrush(Colors.White)
            'DrawWindow.DrawingCanvas.Background.Opacity = 0

            'TODO: can I bind the position of the mouse?
            'BindingHelper.BindSourceToTarget(DrawWindow, DrawWindow.DrawingCanvas.Mou)
            'Mouse.GetPosition(DrawWindow)

        End With

    End Sub

    Private Sub OnMouseWheel(sender As Object, e As MouseWheelEventArgs)

        'TODO: make shift here a variable, that can be set in settings

        Dim CurrentModKeys = Keyboard.Modifiers

        If sender Is DrawWindow.DrawingCanvas Then

            Dim MousePosition = Mouse.GetPosition(DrawWindow)

            If CurrentModKeys And ModifierKeys.Shift Then

                'if it's ONLY shift!
                If (CurrentModKeys - ModifierKeys.Shift) = ModifierKeys.None Then
                    MouseShift(e)
                End If

            ElseIf CurrentModKeys And ModifierKeys.Alt Then

                If (CurrentModKeys - ModifierKeys.Alt) = ModifierKeys.None Then
                    MouseScale(e, MousePosition)
                End If

            End If

        End If

    End Sub

    Private Sub MouseShift(e As MouseWheelEventArgs)

        Const MouseDeltaToScaleFactor As Double = 1

        Dim MouseWheelMovement = e.Delta

        Dim ModelScaleFactor = 1 / (System.Math.Sqrt(ActiveSourceObject.ModelScaler.ScaleFactor))

        Dim CenterShift As Double = MouseWheelMovement / MouseDeltaToScaleFactor * ModelScaleFactor

        ActiveSourceObject.Shift(CenterShift)

    End Sub




    Private Sub MouseScale(e As MouseWheelEventArgs, mousePos As Point)

        'typical Delta for my mouse seems to be 120, need to see how constant this is for different mice / how it can be optimized automatically or have a setting...

        Const MouseDeltaToScaleFactor As Double = 10000

        Dim MouseWheelMovement = e.Delta

        Dim ScaleFactor As Double

        If MouseWheelMovement < 0 Then
            'ScaleFactor=MouseWheelMovement / MouseDeltaToScaleFactor
            ScaleFactor = 1 / 1.1
        Else
            ScaleFactor = 1.1
        End If

        'TODO: add that it zooms centered on mouse position

        ActiveSourceObject.Zoom(ScaleFactor, mousePos)


    End Sub

    Private Sub FDRHardLimitChangeListener(sender As Object, e As PropertyChangedEventArgs)

        If sender Is Parent Then
            If e.PropertyName = NameOf(Parent.FDRHardLimit) Then

                For Each ExistingFDRFilterViewModel In ValueFilterViewModels.Where(Function(x) x.ValueFilter.GetType() Is GetType(FDRFilter))
                    With ExistingFDRFilterViewModel

                        If .ValueFilter.HighLimit <> Parent.FDRHardLimit Then
                            .ValueFilter.HighLimit = Parent.FDRHardLimit
                            .Reset()
                        End If

                    End With
                Next

            End If
        End If
    End Sub

    Public Sub ShowWindow()
        With DrawWindow
            .ShowActivated = True
            .Show()
        End With
    End Sub

    Private Sub DrawSS(sourceObjectViewModel As CrosslinkingSourceObjectViewModel)

        'TODO obviously this should be done in a different way once we have more than one molecule

        Dim TargetHeight As Double = 8

        For Each SS In sourceObjectViewModel.Annotations.SecondaryStructure

            Dim ReferenceUIObject = UIElementsDictionary(sourceObjectViewModel)

            Dim ssView As New Rectangle()

            SS.Shape = ssView

            With ssView

                .Stroke = Nothing
                .StrokeThickness = 0
                .Fill = New SolidColorBrush(SS.Color)
                .Width = (SS.EndIndex - SS.StartIndex) * DrawingScale
                .Height = TargetHeight
                .Opacity = 0.8

            End With

            DrawWindow.DrawingCanvas.Children.Add(ssView)

            Canvas.SetTop(ssView, Canvas.GetTop(ReferenceUIObject) - TargetHeight / 2)
            Canvas.SetLeft(ssView, Canvas.GetLeft(ReferenceUIObject) + SS.StartIndex * DrawingScale)

        Next

    End Sub

    Public Sub DrawSS()

        'TODO obviously this should be done in a different way once we have more than one molecule

        For Each SourceObject In Parent.Data.SourceObjects.Where(Function(x) x.Annotations.SecondaryStructure IsNot Nothing)

            DrawSS(SourceObject)

        Next

    End Sub

    Private Sub RemoveCrosslinks()
        For Each currUI As Windows.UIElement In CrosslinkUICollection
            currUI.Visibility = False
            DrawWindow.DrawingCanvas.Children.Remove(currUI)
        Next
        CrosslinkUICollection.Clear()
    End Sub



    Private Sub UpdateCrosslinkVisibility(o As Object, e As PropertyChangedEventArgs)

        If Not VisibilityFiltersActive Then Exit Sub

        'For all crosslinks that implement a Visibility Filter that links to the object in question
        'Apply the cognate visibility filter


        Dim CrosslinksAttachedToFilter = Parent.Data.Crosslinks.Where(Function(currentCrossLink)
                                                                          Return currentCrossLink.GetVisibilityFilters().
                                                                              Where(Function(filter) filter.SourceValueFilter Is o).Count() > 0
                                                                      End Function).ToList()

        For Each currentCrossLink In CrosslinksAttachedToFilter

            Dim CognateVisibilityFilter = currentCrossLink.GetVisibilityFilters().Where(Function(filter) filter.SourceValueFilter Is o).SingleOrDefault()
            CognateVisibilityFilter.ApplyFilterOn(currentCrossLink)

        Next

    End Sub

    Public Sub DrawCrosslinks()

        'Everything in try block to ensure visibility gets reset in the end

        Dim startingVisibility As Boolean = VisibilityFiltersActive

        Try

            VisibilityFiltersActive = False

            For Each CurrentCrossLink As CrosslinkViewModel In Parent.Data.Crosslinks

                With CurrentCrossLink

                    If Not .IsDrawn Then

                        .DrawMeOnCanvas(DrawWindow.DrawingCanvas, DrawingScale)

                        If .CrossLinkViewUIPath IsNot Nothing Then
                            With .CrossLinkViewUIPath

                                AddHandler .MouseEnter, AddressOf HighlightAndShowCrosslink
                                AddHandler .MouseLeave, AddressOf HideHighlightCrosslink
                                AddHandler .MouseLeftButtonDown, AddressOf ToggleLockHighlight

                            End With
                        End If

                        'TODO: this is dirty, think how to refactor this
                        'Attach all existing value filters to registered visibility filters
                        ValueFilters.ForEach(Sub(y) AddVisibilityFilterToCrosslink(y, CurrentCrossLink))

                    End If

                End With

            Next

        Finally

            VisibilityFiltersActive = startingVisibility

        End Try


    End Sub

    Private Sub AddVisibilityFilterToCrosslink(currentValueFilter As RangeFilter, CurrentCrosslink As CrosslinkViewModel)

        With CurrentCrosslink

            If .SourceObjectViewModel Is currentValueFilter.ReferenceObjectViewModel OrElse
               .TargetObjectViewModel Is currentValueFilter.ReferenceObjectViewModel Then

                CurrentCrosslink.AddVisibilityFilter(New VisibilityFilter(currentValueFilter))

            End If

        End With

    End Sub

    Private Sub AddVisibilityFilterToCrosslink(currentValueFilter As IValueFilter, CurrentCrosslink As CrosslinkViewModel)

        If TypeOf (currentValueFilter) Is RangeFilter Then
            AddVisibilityFilterToCrosslink(CType(currentValueFilter, RangeFilter), CurrentCrosslink)
        Else
            CurrentCrosslink.AddVisibilityFilter(New VisibilityFilter(currentValueFilter))
        End If

    End Sub


    Private Sub ToggleLockHighlight(sender As Object, e As MouseButtonEventArgs)

        If sender IsNot e.OriginalSource Then Exit Sub

        If HighlightLocked Then

            HighlightLocked = False

            RemoveHandler DrawWindow.DrawingCanvas.MouseLeftButtonDown, AddressOf ToggleLockHighlight
            HideHighlightCrosslink(Nothing, Nothing)

        Else

            HighlightLocked = True

            AddHandler DrawWindow.DrawingCanvas.MouseLeftButtonDown, AddressOf ToggleLockHighlight

        End If


    End Sub

    Private Sub HideHighlightCrosslink(sender As Object, e As MouseEventArgs)

        If HighlightLocked Then Exit Sub

        Dim SourcePath As Shape = CType(sender, Shape)

        If HighlightedPath IsNot Nothing Then
            With HighlightedPath
                .StrokeThickness = .StrokeThickness / HighlightThickeningFactor
            End With
        End If

        HighlightedPath = Nothing

        With HighlightElements
            .ForEach(Sub(x) DrawWindow.DrawingCanvas.Children.Remove(x))
            .Clear()
        End With

    End Sub


    Private Sub HighlightAndShowCrosslink(sender As Object, e As MouseEventArgs)

        If HighlightLocked Then Exit Sub

        Dim MinimalSeparation As Double = 5

        HighlightElements = New List(Of UIElement)

        Dim SourcePath As Shape = CType(sender, Shape)
        Dim CognateCrosslink = Parent.Data.Crosslinks.Where(Function(x) x.CrossLinkViewUIPath Is SourcePath).Single()
        Dim ReferenceUIElement = UIElementsDictionary(CognateCrosslink.SourceObjectViewModel)

        Dim StartBox As New TextBox
        Dim EndBox As New TextBox

        HighlightedPath = SourcePath
        SourcePath.StrokeThickness = SourcePath.StrokeThickness * HighlightThickeningFactor

        With HighlightElements
            .Add(StartBox)
            .Add(EndBox)
        End With

        With StartBox

            .Text = CognateCrosslink.SourceObjectViewModel.Sequence(CognateCrosslink.SourceLocus - 1) & CognateCrosslink.SourceLocus
            .Background = New SolidColorBrush(Colors.White)
            .BorderBrush = New SolidColorBrush(Colors.Black)
            DrawWindow.DrawingCanvas.Children.Add(StartBox)
            .Measure(New Size(Double.PositiveInfinity, Double.PositiveInfinity))
            Canvas.SetTop(StartBox, Canvas.GetTop(ReferenceUIElement) - 5)
            Canvas.SetLeft(StartBox, Canvas.GetLeft(ReferenceUIElement) + CognateCrosslink.SourceLocus * DrawingScale - .DesiredSize.Width / 2)
            .IsReadOnly = True

        End With

        With EndBox

            .Text = CognateCrosslink.SourceObjectViewModel.Sequence(CognateCrosslink.TargetLocus - 1) & CognateCrosslink.TargetLocus
            .Background = New SolidColorBrush(Colors.White)
            .BorderBrush = New SolidColorBrush(Colors.Black)
            DrawWindow.DrawingCanvas.Children.Add(EndBox)
            .Measure(New Size(Double.PositiveInfinity, Double.PositiveInfinity))
            Canvas.SetTop(EndBox, Canvas.GetTop(ReferenceUIElement) - 5)
            Canvas.SetLeft(EndBox, Canvas.GetLeft(ReferenceUIElement) + CognateCrosslink.TargetLocus * DrawingScale - .DesiredSize.Width / 2)
            .IsReadOnly = True

        End With

        'make sure the boxes don't overlap in case when the crosslinks are very close to each other
        Dim TextBoxSeparation As Double = (Canvas.GetLeft(EndBox) - Canvas.GetLeft(StartBox) - StartBox.DesiredSize.Width)

        If TextBoxSeparation < MinimalSeparation Then
            Dim TextBoxCorrectingOffset As Double = (MinimalSeparation - TextBoxSeparation) / 2
            Canvas.SetLeft(StartBox, Canvas.GetLeft(StartBox) - TextBoxCorrectingOffset)
            Canvas.SetLeft(EndBox, Canvas.GetLeft(EndBox) + TextBoxCorrectingOffset)
        End If


    End Sub



    Private Function GetUIElement(icrosslinkingSourceObjectViewModel As CrosslinkingSourceObjectViewModel) As UIElement
        Return UIElementsDictionary(icrosslinkingSourceObjectViewModel)
    End Function

    Private Sub UpdateDrawingScale()
        DrawingScale = 0.9 * DrawWindow.DrawingCanvas.ActualWidth / Parent.Data.SourceObjects.Max(Function(x) x.Length)
    End Sub

    Private Sub DrawSliders(currentSourceObjectViewModel As CrosslinkingSourceObjectViewModel)

        'TODO: move FDR filter to global filters

        Dim currentValueFilterViewModels As New List(Of ValueFilterViewModel)

        Dim BottomSliderSpacing As Double = 20

        Dim TemplateUIElement = CType(UIElementsDictionary(currentSourceObjectViewModel), Rectangle)


        Dim MyRangeFilter = New RangeFilter(currentSourceObjectViewModel)
        Dim MyDistanceFilter = New DistanceFilter(currentSourceObjectViewModel)
        Dim MyFDRFilter = New FDRFilter(0, Parent.FDRHardLimit)

        With ValueFilters
            .Add(MyRangeFilter)
            .Add(MyDistanceFilter)
            .Add(MyFDRFilter)
        End With

        Dim RoundingConverter = New DoubleToIntegerConverter
        Dim FDRRoundingConverter = New DoubleRoundingConverter(4)

        Dim NewValueFilterViewModel As ValueFilterViewModel


        'selection of display range
        NewValueFilterViewModel = New ValueFilterViewModel()
        currentValueFilterViewModels.Add(NewValueFilterViewModel)
        With NewValueFilterViewModel

            .ParentCanvas = DrawWindow.AnnotationCanvas
            .ValueFilter = MyRangeFilter

            .Slider = New HorizontalSliderViewModel(
                MyRangeFilter, "RangeLowerFilter", "RangeUpperFilter",
                BindingMode.TwoWay,
                RoundingConverter,
                1, currentSourceObjectViewModel.EndIndex,
                TemplateUIElement.Width)

            .PlaceOnCanvas(TemplateUIElement, 1 * BottomSliderSpacing, 0)
        End With

        'Selection of crosslink distance
        NewValueFilterViewModel = New ValueFilterViewModel()
        currentValueFilterViewModels.Add(NewValueFilterViewModel)
        With NewValueFilterViewModel

            .ParentCanvas = DrawWindow.AnnotationCanvas
            .ValueFilter = MyDistanceFilter

            .Slider = New HorizontalSliderViewModel(
                MyDistanceFilter, "DistanceLowLimit", "DistanceHighLimit",
                BindingMode.TwoWay,
                RoundingConverter,
                1, currentSourceObjectViewModel.EndIndex,
                TemplateUIElement.Width)

            .PlaceOnCanvas(TemplateUIElement, 2 * BottomSliderSpacing, 0)

        End With

        'Selection of FDR
        NewValueFilterViewModel = New ValueFilterViewModel()
        currentValueFilterViewModels.Add(NewValueFilterViewModel)
        ValueFilterViewModels.Add(NewValueFilterViewModel)
        With NewValueFilterViewModel

            .ParentCanvas = DrawWindow.AnnotationCanvas
            .ValueFilter = MyFDRFilter

            .Slider = New HorizontalSliderViewModel(
                MyFDRFilter, "FDRLowLimit", "FDRHighLimit",
                BindingMode.TwoWay,
                FDRRoundingConverter,
                0, Parent.FDRHardLimit,
                TemplateUIElement.Width)

            .PlaceOnCanvas(TemplateUIElement, 3 * BottomSliderSpacing, 0)

        End With

        '****************************************************************
        'Scaling in on a certain region

        Dim ModelScaler = New ScaleController(currentSourceObjectViewModel)

        currentSourceObjectViewModel.ModelScaler = ModelScaler

        Dim NewSlider = New HorizontalSliderViewModel(
            ModelScaler, "ScaleAreaLowLimit", "ScaleAreaHighLimit",
            BindingMode.TwoWay,
            RoundingConverter,
            1, currentSourceObjectViewModel.EndIndex,
            TemplateUIElement.Width)

        NewSlider.PlaceOnCanvas(DrawWindow.DrawingCanvas, Canvas.GetTop(TemplateUIElement) + 4 * BottomSliderSpacing, Canvas.GetLeft(TemplateUIElement))
        AddHandler ModelScaler.PropertyChanged, AddressOf ModelScaler.RedrawObjects

        ValueFilters.ForEach(Sub(x) AddHandler x.PropertyChanged, AddressOf UpdateCrosslinkVisibility)

        VisibilityFiltersActive = True

        'TODO: this is temporary
        currentSourceObjectViewModel.ValueFilterViewModels.AddRange(currentValueFilterViewModels)

    End Sub


    Public Sub DrawTemplate(currentSourceObjectViewModel As CrosslinkingSourceObjectViewModel)

        'TODO: happens automatically on import
        UpdateDrawingScale()

        With DrawWindow.DrawingCanvas

            If Not UIElementsDictionary.ContainsKey(currentSourceObjectViewModel) Then

                Dim TemplateUIElement As New Rectangle()

                With TemplateUIElement

                    .Stroke = Nothing
                    .Fill = Brushes.DarkGray
                    .Opacity = 1

                    .Width = (currentSourceObjectViewModel.EndIndex - currentSourceObjectViewModel.StartIndex) * DrawingScale
                    .Height = 4

                End With

                currentSourceObjectViewModel.Shape = TemplateUIElement

                UIElementsDictionary.Add(currentSourceObjectViewModel, TemplateUIElement)

                .Children.Add(TemplateUIElement)

                Dim NumberOfTemplates = UIElementsDictionary.Count

                Canvas.SetTop(TemplateUIElement, .ActualHeight - 100 * NumberOfTemplates)
                Canvas.SetLeft(TemplateUIElement, (.ActualWidth - TemplateUIElement.Width) / 2)

            End If

        End With
        

        'TODO: take out the drawing of sliders and the modelscaler apart
        DrawSliders(currentSourceObjectViewModel)

        currentSourceObjectViewModel.Adorners.Add(New TemplateNumberScaleViewModel(currentSourceObjectViewModel))
        currentSourceObjectViewModel.Draw(Me)


    End Sub
End Class
