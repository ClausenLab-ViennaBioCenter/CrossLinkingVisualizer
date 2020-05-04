Imports VBExtensions

Public Class DoubleRoundingConverter
    Implements IValueConverter

    Public ReadOnly Property SignificantDigits As Integer

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Return MathExtensions.RoundToSignificantDigits(value, SignificantDigits)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        If value > 0 Then
            Debug.Print(value)
        End If
        Return MathExtensions.RoundToSignificantDigits(value, SignificantDigits)
    End Function

    Public Sub New(iSignificantDigits As Integer)
        SignificantDigits = iSignificantDigits
    End Sub

End Class
