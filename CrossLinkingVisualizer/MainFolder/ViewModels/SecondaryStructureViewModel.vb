Public Class SecondaryStructureViewModel

    Public Sub New(currentChar As Char)
        SSType = currentChar

        Select Case SSType
            Case "H"
                Color = Colors.Red
            Case "E"
                Color = Colors.LimeGreen
            Case Else
                Color = Colors.Black
        End Select

        StrokeWeight = 1

    End Sub

    Public ReadOnly Property SSType As Char

    Public Property EndIndex As Integer
    Public Property StartIndex As Integer

    Public Property Color() As Color

    Public Property Shape As Shape

    Public Property StrokeWeight As Double

    Public Function GetSvgEntry() As String

        Dim colorString As String

        Select Case SSType
            Case "H"
                colorString = "red"
            Case "E"
                colorString = "green"
            Case Else
                colorString = "black"
        End Select

        Return ShapeToSVGConverter.RectangleToSVG(CType(Shape, Rectangle)) ',"none","grey")

    End Function

End Class
