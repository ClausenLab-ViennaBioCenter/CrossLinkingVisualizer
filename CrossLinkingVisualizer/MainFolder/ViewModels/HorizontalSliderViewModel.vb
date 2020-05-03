Public Class HorizontalSliderViewModel

    Public ReadOnly Property Filter As Object
    Public ReadOnly Property Slider As RangeSlider
    Public ReadOnly Property Converter As IValueConverter
    Public ReadOnly Property LowvalueBox As TextBox
    Public ReadOnly Property HighvalueBox As TextBox

    Public Sub PlaceOnCanvas(iCanvas As Canvas, TopPosition As Double, LeftPosition As Double)

        iCanvas.Children.Add(Slider)
        Canvas.SetTop(Slider, TopPosition)
        Canvas.SetLeft(Slider, LeftPosition)

        iCanvas.Children.Add(LowvalueBox)
        Canvas.SetTop(LowvalueBox, TopPosition)
        Canvas.SetLeft(LowvalueBox, LeftPosition - 25)

        iCanvas.Children.Add(HighvalueBox)
        Canvas.SetTop(HighvalueBox, TopPosition)
        Canvas.SetLeft(HighvalueBox, LeftPosition + 5 + Slider.Width)

        'Canvas.SetTop(HighvalueBox, TopPosition + 1 + HighvalueBox.Height)
        'Canvas.SetLeft(HighvalueBox, LeftPosition - HighvalueBox.Width / 2 + Slider.Width)

    End Sub

    Public Sub New(BoundFilter As Object,
                   LowValueFilterPropertyString As String,
                   HighValueFilterPropertyString As String,
                   iBindingMode As BindingMode,
                   iConverter As IValueConverter,
                   Minimum As Double,
                   Maximum As Double,
                   Width As Double)

        Filter = BoundFilter
        Converter = iConverter

        Slider = New RangeSlider
        LowvalueBox = New TextBox
        HighvalueBox = New TextBox

        With Slider
            .Minimum = Minimum
            .Maximum = Maximum
            .Width = Width
        End With

        BindingHelper.BindSourceToTarget(Filter, LowValueFilterPropertyString, Slider, RangeSlider.LowerValueProperty, iBindingMode, iConverter)
        BindingHelper.BindSourceToTarget(Filter, HighValueFilterPropertyString, Slider, RangeSlider.UpperValueProperty, iBindingMode, iConverter)

        BindingHelper.BindSourceToTarget(Filter, LowValueFilterPropertyString, LowvalueBox, TextBox.TextProperty, iBindingMode)
        BindingHelper.BindSourceToTarget(Filter, HighValueFilterPropertyString, HighvalueBox, TextBox.TextProperty, iBindingMode)

    End Sub

End Class
