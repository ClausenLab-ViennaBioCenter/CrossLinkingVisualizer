
Public Class TemplateNumberScaleViewModel
    Implements ICrosslinkingPainterDrawable

    Public Property parentTemplate As CrosslinkingSourceObjectViewModel

    'Public Property MajorTickMarks As List(Of MajorTickMarkViewModel)
    'Public Property MinorTickMarks As List(Of MinorTickMarkViewModel)

    Public Property MainLine As Shape

    Public Property UIElement As Shape
    Public Property textBoxes As List(Of TextBlock)
    
    Private ReadOnly Property NumberScaleStartIndex as Integer
    Get
            Return parentTemplate.ModelScaler.ScaleAreaLowLimit
    End Get
    End Property

    Private ReadOnly Property NumberScaleEndIndex as Integer
    Get
        Return parentTemplate.ModelScaler.ScaleAreaHighLimit
    End Get
    End Property


    Public Sub Draw(iCrossLinkingPainter As CrossLinkingPainter) Implements ICrosslinkingPainterDrawable.Draw
        
        Dim targetCanvas As Canvas = iCrossLinkingPainter.DrawWindow.DrawingCanvas
        Dim DrawingScale As Double = iCrossLinkingPainter.DrawingScale

        Dim MajorTickHeight As Double = 10
        
        Dim StartLeft As Double = Canvas.GetLeft(parentTemplate.Shape)
        Dim StartTop As Double = Canvas.GetTop(parentTemplate.Shape)

        Dim FinalDrawingScale= DrawingScale * parentTemplate.ModelScaler.ScaleFactor

        if Not UIElement Is Nothing then
            targetCanvas.Children.Remove(UIElement)
            textBoxes.ForEach(Sub(x) targetCanvas.Children.Remove(x))
            textBoxes.Clear()
        End If

        Dim originalStartPoint As Point = New Point(StartLeft, StartTop)
        
        Dim pfc As New PathFigureCollection

        'Add main line

        Dim pf As PathFigure = New PathFigure()
        Dim Offset As Vector = New Vector((NumberScaleEndIndex - NumberScaleStartIndex) * FinalDrawingScale, 0)

        With pf
            .StartPoint = originalStartPoint
            .Segments.Add(New LineSegment(.StartPoint + Offset, True))
        End With

        pfc.Add(pf)

        'Add major gridlines

        Dim DesiredMajorIndexSpacing As Integer = CalculateIdealSpacing()

        Dim FirstFittingMajor = RoundToNearestX(NumberScaleStartIndex, DesiredMajorIndexSpacing, RoundingType.RoundUp)

        For i = FirstFittingMajor To NumberScaleEndIndex Step DesiredMajorIndexSpacing

            Offset = New Vector((i - NumberScaleStartIndex) * FinalDrawingScale, -MajorTickHeight / 2)

            Dim StartPoint = originalStartPoint + Offset
            Dim BottomOffset As Vector = New Vector(0, MajorTickHeight)

            pf = New PathFigure()

            With pf
                .StartPoint = StartPoint
                .Segments.Add(New LineSegment(.StartPoint + BottomOffset, True))
            End With

            pfc.Add(pf)

            Dim textLabel As New TextBlock

            With textLabel
                .Text = i
                .FontSize = 10
                .Measure(New Size(Double.PositiveInfinity, Double.PositiveInfinity))
                Canvas.SetLeft(textLabel, StartPoint.X - .DesiredSize.Width / 2)
                Canvas.SetTop(textLabel, StartPoint.Y + .DesiredSize.Height + 1)
                textBoxes.Add(textLabel)
            End With
            
        Next

        If DesiredMajorIndexSpacing >= 5 Then

            Dim MinorStep as Integer = DesiredMajorIndexSpacing \ 5
            Dim MinorTickHeight as double = MajorTickHeight / 2

            'TODO: make this also start / end before the major
            Dim FirstFittingMinor = NumberScaleStartIndex + (FirstFittingMajor - NumberScaleStartIndex) Mod MinorStep

            For i = FirstFittingMinor To NumberScaleEndIndex Step MinorStep

                Offset = New Vector((i - NumberScaleStartIndex) * FinalDrawingScale, -MinorTickHeight / 2)

                Dim StartPoint = originalStartPoint + Offset
                Dim BottomOffset As Vector = New Vector(0, MinorTickHeight)

                pf = New PathFigure()

                With pf
                    .StartPoint = StartPoint
                    .Segments.Add(New LineSegment(.StartPoint + BottomOffset, True))
                End With

                pfc.Add(pf)

            next
                
        End If

        Dim pth As Path = New Path()

        With pth
            .Data = New PathGeometry(pfc)
            .Name = "Numbering"
            .Stroke = Brushes.Black
            .StrokeThickness = 2
        End With

        UIElement = pth

        targetCanvas.Children.Add(UIElement)
        textBoxes.ForEach(Sub(x) targetCanvas.Children.Add(x))

    End Sub

    Private Function CalculateIdealSpacing() As Integer

        Dim MinMarkers As Integer = 5
        Dim MaxMarkers As Integer = 12

        Dim AllowedSpacings As New List(Of Integer) From {1, 5, 25}

        If parentTemplate.ModelScaler.ScaleAreaLength < 25 Then
            Return 1
        End If

        Dim LogScaleFactor As Double

        For i = 0 To Math.Log10(parentTemplate.Length)

            LogScaleFactor = 10 ^ i

            Dim ScaledRange as Integer = parentTemplate.ModelScaler.ScaleAreaLength \ LogScaleFactor

            For Each spacing In AllowedSpacings
                
                Dim NumberOfMarks=ScaledRange \ spacing

                If NumberOfMarks >= MinMarkers AndAlso  NumberOfMarks <=MaxMarkers Then
                    Return spacing*LogScaleFactor
                End If

            Next
            
        Next

    End Function

    Public Sub New(sourceTemplate As CrosslinkingSourceObjectViewModel)

        parentTemplate = sourceTemplate

        textBoxes = New List(Of TextBlock)()

    End Sub

End Class
