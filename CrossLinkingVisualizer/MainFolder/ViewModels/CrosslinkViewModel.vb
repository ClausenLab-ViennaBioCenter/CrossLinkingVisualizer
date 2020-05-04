Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.Serialization.Formatters

Public Class CrosslinkViewModel
    Implements INotifyPropertyChanged, IDisposable

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public ReadOnly Property SourceObjectViewModel As CrosslinkingSourceObjectViewModel
    Public ReadOnly Property TargetObjectViewModel As CrosslinkingSourceObjectViewModel

    Public ReadOnly Property SourceLocus As Integer
    Public ReadOnly Property TargetLocus As Integer

    Public ReadOnly Property Distance As Integer
        Get
            Return TargetLocus - SourceLocus
        End Get
    End Property

    Private Property VisibilityFilters As ObservableCollection(Of VisibilityFilter)

    'TODO: make this a readonly class, we don't want the outside world to be able to circumvent the addhandler! It should only be possible to add through addVisibilityFIlter method
    Public Function GetVisibilityFilters() As ObservableCollection(Of VisibilityFilter)
        Return VisibilityFilters
    End Function

    Public Sub AddVisibilityFilter(iVisibilityFilter As VisibilityFilter)
        If VisibilityFilters().Select(Function(x) x.SourceValueFilter).Contains(iVisibilityFilter.SourceValueFilter) Then
            'TODO: handle this better
            Debug.Print("Tried to add a duplicate of an existing filter")
            Exit Sub
            'Throw New ArgumentException("Tried to add a duplicate of an existing filter")
        Else
            AddHandler iVisibilityFilter.PropertyChanged, AddressOf UpdateVisibility
            VisibilityFilters.Add(iVisibilityFilter)
        End If
    End Sub

    Private _ShouldBeVisible As Boolean
    Public Property ShouldBeVisible As Boolean
        Get
            Return _ShouldBeVisible
        End Get
        Set
            _ShouldBeVisible = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("ShouldBeVisible"))
        End Set
    End Property

    Private _weight As Double
    Public Property Weight As Double
        Get
            Return _weight
        End Get
        Set(value As Double)
            _weight = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Weight"))
        End Set
    End Property


    Private _group As String
    Public Property Group As String
        Get
            Return _group
        End Get
        Set(value As String)
            _group = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Group"))
        End Set
    End Property

    Public Property FDR As Double
    Public Property PSMCount As Integer
    Public Property PeptidePairCount As Integer

    Public Property CrossLinkViewUIPath As Shape

    Public ReadOnly Property IsDrawn As Boolean
        Get
            Return CrossLinkViewUIPath IsNot Nothing
        End Get
    End Property

    Private Sub InitializeMe()

        AddHandler GetVisibilityFilters().CollectionChanged, AddressOf UpdateVisibility

    End Sub

    Private Sub UpdateVisibility()

        If ShouldBeVisible Then

            For Each CurrentFilter In GetVisibilityFilters()
                If Not CurrentFilter.ShouldBeVisible Then
                    ShouldBeVisible = False
                    Exit Sub
                End If
            Next

        Else

            For Each CurrentFilter In GetVisibilityFilters()
                If Not CurrentFilter.ShouldBeVisible Then
                    Exit Sub
                End If
            Next
            'if we reach this point, it means all the filters are set to visible
            ShouldBeVisible = True

        End If

    End Sub

    Public Sub New(iSourceObjectViewModel As CrosslinkingSourceObjectViewModel, iTargetObjectViewModel As CrosslinkingSourceObjectViewModel,
                   iSourceLocus As Integer, iTargetLocus As Integer,
                   iGroup As String)

        _VisibilityFilters = New ObservableCollection(Of VisibilityFilter)
        ShouldBeVisible = True

        SourceObjectViewModel = iSourceObjectViewModel
        TargetObjectViewModel = iTargetObjectViewModel

        SourceLocus = iSourceLocus
        TargetLocus = iTargetLocus

        Group = iGroup
        Weight = 1

        InitializeMe()

    End Sub

    Private Shared ReadOnly Property BoolToVisConverter
        Get
            Return New BooleanToVisibilityConverter
        End Get

    End Property

    Public ReadOnly Property IsInternal As Boolean
        Get
            Return SourceObjectViewModel Is TargetObjectViewModel
        End Get
    End Property



    Public Function GetEndPosition(targetCanvas As Canvas, DrawingScale As Double) As Point

        Dim SourceUIElement As UIElement = TargetObjectViewModel.Shape

        Dim SourceObjectStart As Point
        Dim OffsetWithinObject As Vector

        With targetCanvas
            SourceObjectStart = New Point(Canvas.GetLeft(SourceUIElement), Canvas.GetTop(SourceUIElement))
        End With

        OffsetWithinObject = New Vector((TargetLocus - TargetObjectViewModel.StartIndex) * DrawingScale, 0)


        Return SourceObjectStart + OffsetWithinObject

    End Function

    Public Function GetStartPosition(targetCanvas As Canvas, DrawingScale As Double) As Point

        Dim SourceUIElement As UIElement = SourceObjectViewModel.Shape

        Dim SourceObjectStart As Point
        Dim OffsetWithinObject As Vector

        With targetCanvas
            SourceObjectStart = New Point(Canvas.GetLeft(SourceUIElement), Canvas.GetTop(SourceUIElement))
            'SourceObjectStart = New Point(SourceUIElement.Marg, .GetTop(SourceUIElement))
        End With

        OffsetWithinObject = New Vector((SourceLocus - TargetObjectViewModel.StartIndex) * DrawingScale, 0)

        Return SourceObjectStart + OffsetWithinObject

    End Function


    Friend Sub DrawMeOnCanvas(targetCanvas As Canvas, DrawingScale As Double)

        'TODO: remove the Me
        'TODO: add logic for different drawing at different distance between crosslinks?

        With Me

            Dim startPosition As Point
            Dim endPosition As Point

            If .SourceObjectViewModel Is .TargetObjectViewModel Then

                If .SourceLocus <> .TargetLocus Then

                    startPosition = GetStartPosition(targetCanvas, DrawingScale)
                    endPosition = GetEndPosition(targetCanvas, DrawingScale)

                    Dim HorizontalStretch = New Vector(endPosition.X - startPosition.X, 0)

                    Dim arc As ArcSegment = New ArcSegment(
                        startPosition + HorizontalStretch,
                            New Size(System.Math.Abs(HorizontalStretch.X) / 2, System.Math.Abs(HorizontalStretch.X) / 2),
                            0,
                            False,
                            SweepDirection.Clockwise,
                            True)

                    Dim pf As PathFigure = New PathFigure()
                    pf.Segments.Add(arc)
                    pf.StartPoint = startPosition

                    Dim pfc As New PathFigureCollection From {pf}
                    Dim pg As New PathGeometry(pfc)

                    Dim pth As Path = New Path()

                    With pth
                        .Data = pg
                        .Name = "XL_" & SourceLocus & "_" & TargetLocus
                    End With

                    pth.Stroke = Brushes.Black
                    targetCanvas.Children.Add(pth)

                    'bind the "should be visible" of each CrosslinkViewModel object to visibility of its respective UI element
                    BindingHelper.BindSourceToTarget(Me, "ShouldBeVisible", pth, Path.VisibilityProperty, BindingMode.OneWay, BoolToVisConverter)
                    CrossLinkViewUIPath = pth

                End If

            Else

                Dim line As Line = New Line()

                startPosition = GetStartPosition(targetCanvas, DrawingScale)
                endPosition = GetEndPosition(targetCanvas, DrawingScale)

                With line
                    .X1 = startPosition.X
                    .X2 = endPosition.X
                    .Y1 = startPosition.Y
                    .Y2 = endPosition.Y

                    .Name = "XL_" & SourceLocus & "_" & TargetLocus

                    .Stroke = Brushes.Black
                End With

                targetCanvas.Children.Add(line)

                'bind the "should be visible" of each CrosslinkViewModel object to visibility of its respective UI element
                BindingHelper.BindSourceToTarget(Me, "ShouldBeVisible", line, Path.VisibilityProperty, BindingMode.OneWay, BoolToVisConverter)
                CrossLinkViewUIPath = line

                'Throw New NotImplementedException("So far only internal crosslinks in a single model is supported")

            End If



        End With

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

    Public Function GetSvgEntry() As String

        Select Case True
            Case TypeOf CrossLinkViewUIPath Is Line
                Return ShapeToSVGConverter.ShapeToSVG(CType(CrossLinkViewUIPath, Line))
            Case TypeOf CrossLinkViewUIPath Is Path
                Return ShapeToSVGConverter.ShapeToSVG(CType(CrossLinkViewUIPath, Path))
            Case Else
                Throw New NotImplementedException
        End Select

    End Function

End Class
