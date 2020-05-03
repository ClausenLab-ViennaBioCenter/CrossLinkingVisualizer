Imports System.Collections.ObjectModel

Public Class CrosslinkingData

    Public Property SourceObjects() As ObservableCollection(Of CrosslinkingSourceObjectViewModel)
    Public Property Crosslinks() As ObservableCollection(Of CrosslinkViewModel)

    Public Sub New()

        SourceObjects = New ObservableCollection(Of CrosslinkingSourceObjectViewModel)
        Crosslinks = New ObservableCollection(Of CrosslinkViewModel)

    End Sub

End Class
