Imports System.ComponentModel
Imports System.Globalization

Public Class ShapeToSVGConverter

    Shared ReadOnly Property converterCulture As CultureInfo = CultureInfo.InvariantCulture

    Shared Function RectangleToSVG(refShape As Rectangle) As String

        Dim strokeColor, fillColor As String

        If refShape.Stroke Is Nothing Then
            strokeColor = "none"
        Else
            strokeColor = SVGColorConverter(refShape.Stroke)
        End If

        If refShape.Fill Is Nothing Then
            fillColor = "none"
        Else
            fillColor = SVGColorConverter(refShape.Fill)
        End If


        Return RectangleToSVG(refShape, strokeColor, fillColor)

    End Function

    Public Shared Function SVGColorConverter(inputBrush As Brush) As String

        Dim myColor = CType(inputBrush, SolidColorBrush).Color

        With myColor
            Return String.Format("rgba({0},{1},{2},{3})", .R, .G, .B, (.A / Byte.MaxValue).ToString(converterCulture))
        End With

    End Function



    Public Shared Function RectangleToSVG(refShape As Rectangle, strokeColor As String, fillColor As String) As String

        Dim startX = Canvas.GetLeft(refShape)
        Dim startY = Canvas.GetTop(refShape)

        Dim pathData As String = String.Format("M{0},{1}h{2}v{3}h-{2}v-{3}", startX.ToString(converterCulture), startY.ToString(converterCulture), refShape.Width.ToString(converterCulture), refShape.Height.ToString(converterCulture))

        Dim a As Geometry = (New GeometryConverter).ConvertFromString(pathData)
        Return String.Format("<path d=""{0}"" stroke-width=""{1}"" stroke=""{2}"" fill=""{3}""/>",
                            a.ToString().Replace(",", ".").Replace(";", " "),
                            refShape.StrokeThickness.ToString(converterCulture),
                            strokeColor,
                            fillColor)

    End Function

    Public Shared Function ShapeToSVG(refShape As Line) As String

        Dim strokeColor, fillColor As String

        If refShape.Stroke Is Nothing Then
            strokeColor = "none"
        Else
            strokeColor = SVGColorConverter(refShape.Stroke)
        End If

        If refShape.Fill Is Nothing Then
            fillColor = "none"
        Else
            fillColor = SVGColorConverter(refShape.Fill)
        End If

        Dim startX = Canvas.GetLeft(refShape)
        Dim startY = Canvas.GetTop(refShape)

        Dim pathData As String

        With refShape
            pathData = String.Format("M{0},{1}L{2},{3}",
                                               .X1.ToString(converterCulture),
                                               .Y1.ToString(converterCulture),
                                                   .X2.ToString(converterCulture),
                                                   .Y2.ToString(converterCulture))
        End With

        Dim a As Geometry = (New GeometryConverter).ConvertFromString(pathData)
        Return String.Format("<path d=""{0}"" stroke-width=""{1}"" stroke=""{2}"" fill=""{3}""/>",
                             a.ToString().Replace(",", ".").Replace(";", " "),
                             refShape.StrokeThickness.ToString(converterCulture),
                             strokeColor,
                             fillColor)

    End Function


    Public Shared Function ShapeToSVG(crossLinkViewUiPath As Path) As String

        Return String.Format("<path d=""{0}"" stroke=""{2}"" stroke-width=""{1}"" fill=""none""/>",
                             CType(crossLinkViewUiPath, Path).Data.ToString.Replace(",", ".").Replace(";", " "),
                             crossLinkViewUiPath.StrokeThickness,
                             SVGColorConverter(crossLinkViewUiPath.Stroke))
    End Function
End Class
