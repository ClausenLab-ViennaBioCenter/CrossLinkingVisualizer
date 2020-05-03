Imports System.ComponentModel

Public Class ScaleController
    Implements INotifyPropertyChanged

    Private _ScaleAreaLowLimit As Double
    Private _ScaleAreaHighLimit As Double
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private ScaleToRangeLink As Boolean

    Public Property ScaleAreaLowLimit As Double
        Get
            Return _ScaleAreaLowLimit
        End Get
        Set
            Value = Math.Round(Value, 0)
            If Value <> ScaleAreaLowLimit Then

                If Value >= SourceObjectViewModel.EndIndex Then
                    Value = SourceObjectViewModel.EndIndex
                ElseIf Value <= SourceObjectViewModel.StartIndex Then
                    Value = SourceObjectViewModel.StartIndex
                End If

                _ScaleAreaLowLimit = Value

                If ScaleToRangeLink Then

                    ScaleToRangeLink = False
                    UpdateScale()
                    ScaleToRangeLink = True

                End If

                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ScaleAreaLowLimit)))

            End If
        End Set
    End Property


    Public Property ScaleAreaHighLimit As Double
        Get
            Return _ScaleAreaHighLimit
        End Get
        Set
            If Value <> ScaleAreaHighLimit Then
                Value = Math.Round(Value, 0)

                If Value >= SourceObjectViewModel.EndIndex Then
                    Value = SourceObjectViewModel.EndIndex
                ElseIf Value <= SourceObjectViewModel.StartIndex Then
                    Value = SourceObjectViewModel.StartIndex
                End If

                _ScaleAreaHighLimit = Value

                If ScaleToRangeLink Then

                    ScaleToRangeLink = False
                    UpdateScale()
                    ScaleToRangeLink = True

                End If

                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ScaleAreaHighLimit)))

            End If
        End Set
    End Property

    Private Sub UpdateScale()

        ScaleFactor = ScaleFactor
        ScaleCenter = ScaleCenter

        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ScaleFactor)))

    End Sub


    Private Sub UpdateLimits(targetScale As Double, targetCenter As Double)

        With SourceObjectViewModel

            If targetScale = 1 Then

                ScaleAreaLowLimit = .StartIndex
                ScaleAreaHighLimit = .EndIndex

            Else

                ScaleAreaLowLimit = (.StartIndex + (targetScale - 1) * targetCenter) / targetScale
                ScaleAreaHighLimit = .Length / targetScale + (ScaleAreaLowLimit - 1)

            End If

        End With

        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ScaleFactor)))

    End Sub



    Private SavedScaleFactor As Double
    Public Property ScaleFactor As Double
        Get
            Return MaxScaleAreaLength / ScaleAreaLength
        End Get
        Set(value As Double)
            If value <> SavedScaleFactor Then

                If Not ScaleValid(value) Then
                    Exit Property
                    Throw New ArgumentException("Invalid center", NameOf(ScaleFactor))
                End If
                SavedScaleFactor = value

                If ScaleToRangeLink Then

                    ScaleToRangeLink = False
                    UpdateLimits(SavedScaleFactor, SavedCenter)
                    ScaleToRangeLink = True

                End If

                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ScaleFactor)))

            End If
        End Set
    End Property

    Private Function ScaleValid(value As Double) As Boolean
        'TODO: more checks, e.g. when scaling you shouldn't be able to change the centre?
        If value >= MinimumScale AndAlso value <= MaximumScale Then
            Return True
        Else
            Return False
        End If
    End Function

    Private ReadOnly Property MaximumScale As Double
        Get
            Return MaxScaleAreaLength
        End Get
    End Property

    Private ReadOnly Property MinimumScale As Double
        Get
            Return 1
        End Get
    End Property

    Function CalculateScaleCenter()
        Return (ScaleAreaLowLimit * ScaleFactor - SourceObjectViewModel.StartIndex) / (ScaleFactor - 1)
    End Function

    Private SavedCenter As Double
    'Bear in mind this is the _SCALE_ center, which changes as you zoom and is NOT in the middle of the sequence! It's based on the top left corner for most objects.
    Public Property ScaleCenter As Double
        Get

            'formula determined painstakingly and finally on 2018-09-19 22:42
            ' Return (ScaleAreaLowLimit + ScaleAreaHighLimit) / 2
            If ScaleFactor = 1 Then
                ScaleCenter = SourceObjectViewModel.StartIndex
            Else
                Return CalculateScaleCenter()
            End If



        End Get
        Set(value As Double)
            value = Math.Round(value, 0)
            If value <> SavedCenter Then

                If Not CenterValid(value) Then
                    If value < MinCenter Then
                        value = MinCenter
                    ElseIf value > MaxCenter Then
                        value = MaxCenter
                    End If
                    'Throw New ArgumentException("Invalid center", NameOf(ScaleCenter))
                End If

                SavedCenter = value

                If ScaleToRangeLink Then

                    ScaleToRangeLink = False
                    UpdateLimits(SavedScaleFactor, SavedCenter)
                    ScaleToRangeLink = True

                End If

                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ScaleCenter)))

            End If
        End Set
    End Property

    Private ReadOnly Property MinCenter As Double
        Get
            Return SourceObjectViewModel.StartIndex
        End Get
    End Property

    Private ReadOnly Property MaxCenter As Double
        Get
            Return MaxHighLimit + 1
        End Get
    End Property

    Private Function CenterValid(value As Double) As Boolean
        'TODO: implement controls for valid arguments
        'math has spoken - max reasonable Center, when lowLimit=highLimit, is actually highLimit + 1 !
        'tolerance is needed because of rounding errors
        Dim tolerance As Double = 1.001
        If value >= MaxLowLimit / tolerance AndAlso value <= MaxCenter * tolerance Then
            Return True
        Else
            Return False
        End If
    End Function


    Private ReadOnly Property MaxScaleAreaLength As Double
        Get
            Return MaxHighLimit - MaxLowLimit + 1
        End Get
    End Property

    Private ReadOnly Property MaxLowLimit As Double
        Get
            Return SourceObjectViewModel.StartIndex
        End Get
    End Property

    Private ReadOnly Property MaxHighLimit As Double
        Get
            Return SourceObjectViewModel.EndIndex
        End Get
    End Property

    Public Property ScaleAreaLength As Double
        Get
            Return ScaleAreaHighLimit - ScaleAreaLowLimit + 1
        End Get
        Set(value As Double)
            If value >= 1 AndAlso value <= MaxScaleAreaLength Then
                Throw New NotImplementedException
            End If
        End Set
    End Property

    Public ReadOnly Property SourceObjectViewModel as CrosslinkingSourceObjectViewModel

    'TODO: this should probably not be in here
    Public Property TargetScaleTransform as ScaleTransform

    Public Sub New(crosslinkingSourceObjectViewModel As CrosslinkingSourceObjectViewModel)

        SourceObjectViewModel = crosslinkingSourceObjectViewModel

        ScaleToRangeLink = False

        ScaleAreaLowLimit = SourceObjectViewModel.StartIndex
        ScaleAreaHighLimit = SourceObjectViewModel.EndIndex
        'TargetScaleTransform= New ScaleTransform(1,1,)

        UpdateScale()

        ScaleToRangeLink = True

    End Sub

    Public Sub ShiftCenter(shiftAmount As Double)

        If Math.Abs(shiftAmount) > 0 Then

            Dim finalShift As Double

            If shiftAmount > 0 Then

                Dim MaxPositiveShift = MaxHighLimit - ScaleAreaHighLimit
                finalShift = Math.Min(MaxPositiveShift, shiftAmount)

                'start with high, to prevent the possibility of low "overshooting" it
                ScaleToRangeLink = False
                ScaleAreaHighLimit += shiftAmount
                ScaleToRangeLink = True
                ScaleAreaLowLimit += shiftAmount

            Else

                Dim MaxNegativeShift = MaxLowLimit - ScaleAreaLowLimit
                finalShift = System.Math.Max(MaxNegativeShift, shiftAmount)

                'start with low, to prevent the possibility of high "overshooting" it
                ScaleToRangeLink = False
                ScaleAreaLowLimit += shiftAmount
                ScaleToRangeLink = True
                ScaleAreaHighLimit += shiftAmount

            End If

        End If

    End Sub

    Friend Sub ScaleCenteredOn(extraScaleFactor As Double, targetScaleCenter As Double)

        If System.Math.Round(extraScaleFactor, 4) <> 1 Then

            If targetScaleCenter > MaxCenter Then
                targetScaleCenter = MaxCenter
            ElseIf targetScaleCenter < MinCenter Then
                targetScaleCenter = MinCenter
            End If

            Dim FinalScale As Double

            If extraScaleFactor > 1 Then

                Dim MaxScaleMultiplier = ScaleFactor / MinimumScale
                FinalScale = Math.Min(MaxScaleMultiplier, extraScaleFactor)

                'start with high, to prevent the possibility of low "overshooting" it
                ScaleToRangeLink = False
                ScaleAreaHighLimit = targetScaleCenter + (ScaleAreaHighLimit - SavedCenter) * FinalScale
                ScaleToRangeLink = True
                ScaleAreaLowLimit = targetScaleCenter + (ScaleAreaLowLimit - SavedCenter) * FinalScale

            Else

                Dim MinScaleMultiplier = ScaleFactor / MaximumScale
                FinalScale = Math.Max(MinScaleMultiplier, extraScaleFactor)

                'start with high, to prevent the possibility of low "overshooting" it
                ScaleToRangeLink = False
                ScaleAreaHighLimit = targetScaleCenter + (ScaleAreaHighLimit - SavedCenter) * FinalScale
                ScaleToRangeLink = True
                ScaleAreaLowLimit = targetScaleCenter + (ScaleAreaLowLimit - SavedCenter) * FinalScale

            End If



        End If

    End Sub


    Public Sub RedrawObjects()

        'TODO: make only template object get changed here, and it propagates its changes to its children objects (SS, crosslink)

        Dim ReferenceShape = SourceObjectViewModel.Shape

        Dim ScaleChange As Double = ScaleFactor / SourceObjectViewModel.ScaleScaleFactor

        'most object scale from the top left, and my start index always starts there for now
        Dim DefaultScaleCenter = SourceObjectViewModel.StartIndex

        Dim ScaleCenterShift As Double = (ScaleCenter - DefaultScaleCenter) * SourceObjectViewModel.Parent.DrawingScale

        'TEMP
        'ScaleCenterShift = SourceObjectViewModel.Length / 2 * DrawingScale

        'Template Object
        ReferenceShape.RenderTransform = New ScaleTransform(ScaleFactor, 1, ScaleCenterShift, 0)
        'TODO:refactor this
        SourceObjectViewModel.DrawAdorners()
        'Debug.Print("A:"&Canvas.GetLeft(ReferenceShape))

        'Secondary structure
        For Each currentSSViewModel In SourceObjectViewModel.Annotations.SecondaryStructure
            currentSSViewModel.Shape.RenderTransform = New ScaleTransform(ScaleFactor, 1,
            Canvas.GetLeft(ReferenceShape) - Canvas.GetLeft(currentSSViewModel.Shape) + ScaleCenterShift, 0)
            currentSSViewModel.Shape.StrokeThickness = currentSSViewModel.StrokeWeight / ScaleFactor
        Next

        'Domains
        For Each currentDomain In SourceObjectViewModel.Annotations.Domains
            With currentDomain
                .Shape.RenderTransform = New ScaleTransform(ScaleFactor, 1,
                Canvas.GetLeft(ReferenceShape) - Canvas.GetLeft(.Shape) + ScaleCenterShift, 0)
                .Shape.StrokeThickness = .StrokeWeight / ScaleChange
            End With
        Next

        'Crosslinks
        For Each currentCrosslink In SourceObjectViewModel.Parent.Parent.Data.Crosslinks
            With currentCrosslink.CrossLinkViewUIPath
                'TODO: I HAVE NO FUCKING IDEA WHY THIS WORKS AND I SPEND 3 HOURS FIGURING IT OUT
                'TODO: WHY WOULD THE OFFSET BE THIS? THAT MEANS CROSSLINKS ARE BEING SCALED FROM THE WINDOW BOUNDS?
                'so... it's because crosslinks are path objects and don't have a "left, so they scale based on something external I guess..
                Dim Offset = Canvas.GetLeft(ReferenceShape)
                'Debug.Print (.Data.Bounds.Left)
                .StrokeThickness = currentCrosslink.Weight / ScaleFactor
                '.RenderTransform= new ScaleTransform(ScaleFactor, 1, Offset + ScaleCenterShift, 0) - just the X-scaling
                .RenderTransform = New ScaleTransform(ScaleFactor, ScaleFactor, Offset + ScaleCenterShift, Canvas.GetTop(ReferenceShape))
            End With
        Next

    End Sub

End Class
