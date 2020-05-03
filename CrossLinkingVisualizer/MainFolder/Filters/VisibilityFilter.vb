Imports System.ComponentModel
Imports CrossLinkingVisualizer

Public Class VisibilityFilter
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property SourceValueFilter As IValueFilter

    Private _ShouldBeVisible As Boolean
    Public Property ShouldBeVisible As Boolean
        Get
            Return _ShouldBeVisible
        End Get
        Set
            _ShouldBeVisible = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("ShouldBeVisible"))
        End Set
    End Property

    Public Sub New(iSourceObject As IValueFilter)

        SourceValueFilter = iSourceObject
        ShouldBeVisible = True

    End Sub

    Friend Sub ApplyFilterOn(currentCrossLink As CrosslinkViewModel)

        Dim NewValueShouldBeVisible As Boolean = SourceValueFilter.CheckFilter(currentCrossLink)

        'Only update if it's not already correct, to prevent unneccessary triggering of INotifyPropertyChanged
        If NewValueShouldBeVisible Then
            If Not ShouldBeVisible Then ShouldBeVisible = True
        Else
            If ShouldBeVisible Then ShouldBeVisible = False
        End If

    End Sub
End Class
