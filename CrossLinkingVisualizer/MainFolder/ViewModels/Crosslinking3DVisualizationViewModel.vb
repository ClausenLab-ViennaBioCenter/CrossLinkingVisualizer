Imports System.ComponentModel
Imports CrossLinkingVisualizer

Public Class Crosslinking3DVisualizationViewModel
    Implements ComponentModel.INotifyPropertyChanged

    Private sourceObject As CrosslinkingSourceObjectViewModel

    Public Sub New(sourceObject As CrosslinkingSourceObjectViewModel)
        Me.SourceCrosslinkingObject = sourceObject
        Me.MolecularModels = New ObjectModel.ObservableCollection(Of MolecularModelViewModel)
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property SourceCrosslinkingObject As CrosslinkingSourceObjectViewModel
    Public Property MolecularModels As ObjectModel.ObservableCollection(Of MolecularModelViewModel)

End Class
