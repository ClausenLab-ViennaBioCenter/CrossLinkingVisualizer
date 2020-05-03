Public Class StartPositionConverter
    Implements IValueConverter

    Private Property BaseUI As UIElement


    Public Sub New(BaseObject As UIElement)
        BaseUI = BaseObject
    End Sub

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Throw New NotImplementedException()
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function
End Class