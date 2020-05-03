Imports System.ComponentModel
Imports CrossLinkingVisualizer

Public Class FDRFilter
    Implements IValueFilter

    Private _FDRLowLimit As Double
    Private _FDRHighLimit As Double
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property FDRLowLimit As Double Implements IValueFilter.LowLimit
        Get
            Return _FDRLowLimit
        End Get
        Set
            _FDRLowLimit = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(FDRLowLimit)))
        End Set
    End Property

    Public Property FDRHighLimit As Double Implements IValueFilter.HighLimit
        Get
            Return _FDRHighLimit
        End Get
        Set
            _FDRHighLimit = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(FDRHighLimit)))
        End Set
    End Property

    Public Sub New(lowLimit As Double, highLimit As Double)
        FDRLowLimit = lowLimit
        FDRHighLimit = highLimit
    End Sub


    Public Function CheckFilter(crosslinkViewModel As CrosslinkViewModel) As Boolean Implements IValueFilter.CheckFilter

        Return System.Math.Abs(crosslinkViewModel.FDR) >= FDRLowLimit AndAlso
           System.Math.Abs(crosslinkViewModel.FDR) <= FDRHighLimit

    End Function

End Class


