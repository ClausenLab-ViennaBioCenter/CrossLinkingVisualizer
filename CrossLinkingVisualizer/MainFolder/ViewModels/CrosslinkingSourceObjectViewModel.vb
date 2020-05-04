Imports System.ComponentModel

Public Class CrosslinkingSourceObjectViewModel
    Implements INotifyPropertyChanged
    Implements ICrosslinkingPainterDrawable

    Public Sub New(name As String, sequence As String, startIndex As Integer, endIndex As Integer, parent As CrossLinkingPainter)

        Me.Parent = parent

        Me.StartIndex = startIndex
        Me.EndIndex = endIndex
        Me.Name = name
        Me.Sequence = sequence
        Me.Annotations = New CrossLinkingObjectAnnotations()
        Me.ScaleOriginIndexOffset = 0
        Me.ScaleScaleFactor = 1

        Me.Adorners = New List(Of ICrosslinkingPainterDrawable)()

        ValueFilterViewModels = New List(Of ValueFilterViewModel)()

    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property Parent As CrossLinkingPainter

    Public Property StartIndex() As Integer
    Public Property EndIndex() As Integer

    Public Property Name As String
    Public Property Sequence As String

    Public Property ModelScaler As ScaleController

    Public ReadOnly Property DrawingScale As Double
        Get
            Return Parent.DrawingScale
        End Get
    End Property

    Public Property Adorners As List(Of ICrosslinkingPainterDrawable)

    Public ReadOnly Property Length() As Integer
        Get
            Return 1 + EndIndex - StartIndex
        End Get
    End Property

    Public Property Annotations As CrossLinkingObjectAnnotations

    Public Function ValidateIndex(sourceIndex As Integer) As Boolean
        Return sourceIndex >= StartIndex AndAlso sourceIndex <= EndIndex
    End Function

    Public Property ScaleOriginIndexOffset As Double

    Public Property ScaleScaleFactor() As Double

    Public Property Shape As Shape
    
    Public Property ValueFilterViewModels as List(Of ValueFilterViewModel)

    Friend Sub Shift(centerShift As Double)
        ModelScaler.ShiftCenter(centerShift)
    End Sub

    Public Sub Zoom(scaleFactor As Double, mousePosition As Point)

        'TODO: implement
        'ModelScaler.ScaleCenteredOn(scaleFactor, mousePosition)

        ModelScaler.ScaleCenteredOn(scaleFactor, ModelScaler.ScaleCenter)

    End Sub

    Friend Sub DrawAdorners()
        Adorners.ForEach(Sub(x) x.Draw(Parent))
    End Sub

    Public Sub Draw(iCrossLinkingParent As CrossLinkingPainter) Implements ICrosslinkingPainterDrawable.Draw
        DrawAdorners()
    End Sub

    Public Function GetSvgEntry() As String

        Return ShapeToSVGConverter.RectangleToSVG(CType(Shape, Rectangle))

    End Function

End Class
