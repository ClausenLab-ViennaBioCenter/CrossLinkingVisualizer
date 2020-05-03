Imports System.ComponentModel
'Imports System.Windows.DependencyObject

Public Class DistanceFilter
    Implements IValueFilter

    Private _DistanceLowLimit As Double
    Private _DistanceHighLimit As Double
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property DistanceLowLimit As Double Implements IValueFilter.LowLimit
        Get
            Return _DistanceLowLimit
        End Get
        Set
            _DistanceLowLimit = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DistanceLowLimit)))
        End Set
    End Property

    Public Property DistanceHighLimit As Double Implements IValueFilter.HighLimit
        Get
            Return _DistanceHighLimit
        End Get
        Set
            _DistanceHighLimit = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DistanceHighLimit)))
        End Set
    End Property

    Public Sub New(iSourceObjectViewModel As CrosslinkingSourceObjectViewModel)
        DistanceLowLimit = 1
        DistanceHighLimit = iSourceObjectViewModel.Length
    End Sub

    Public Function CheckFilter(crosslinkViewModel As CrosslinkViewModel) As Boolean Implements IValueFilter.CheckFilter

        Return System.Math.Abs(crosslinkViewModel.SourceLocus - crosslinkViewModel.TargetLocus) >= DistanceLowLimit AndAlso
               System.Math.Abs(crosslinkViewModel.SourceLocus - crosslinkViewModel.TargetLocus) <= DistanceHighLimit

    End Function

End Class
