Public Class ValueFilterViewModel

    Public Property ValueFilter as IValueFilter
    Public Property Slider as HorizontalSliderViewModel
    Public Property ParentCanvas As Canvas


    Friend Sub Reset()

        With Slider.Slider
            .LowerValue = .Minimum
            .UpperValue = .Maximum
        End With

    End Sub

    Public Sub PlaceOnCanvas(templateUiElement As UIElement, offsetTop As Double, offsetLeft As Double)
        Slider.PlaceOnCanvas(ParentCanvas, Canvas.GetTop(templateUiElement) + offsetTop, Canvas.GetLeft(templateUiElement) + offsetLeft)
    End Sub

End Class
