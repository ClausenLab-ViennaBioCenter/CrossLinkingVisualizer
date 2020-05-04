Imports System.Collections.ObjectModel

Public Class CrosslinkingData
    Implements ComponentModel.INotifyPropertyChanged

    Public Event PropertyChanged As ComponentModel.PropertyChangedEventHandler Implements ComponentModel.INotifyPropertyChanged.PropertyChanged

    Public Property SourceObjects() As ObservableCollection(Of CrosslinkingSourceObjectViewModel)
    Public Property Crosslinks() As ObservableCollection(Of CrosslinkViewModel)

    Public Sub New()

        SourceObjects = New ObservableCollection(Of CrosslinkingSourceObjectViewModel)
        Crosslinks = New ObservableCollection(Of CrosslinkViewModel)

        AddHandler SourceObjects.CollectionChanged, AddressOf OnDataChanged

    End Sub

    Private Sub OnDataChanged(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs)

        If sender Is SourceObjects Then
            RaiseEvent PropertyChanged(sender, New ComponentModel.PropertyChangedEventArgs(NameOf(SourceObjects)))
        ElseIf sender Is Crosslinks Then
            RaiseEvent PropertyChanged(sender, New ComponentModel.PropertyChangedEventArgs(NameOf(Crosslinks)))
        End If
    End Sub

End Class
