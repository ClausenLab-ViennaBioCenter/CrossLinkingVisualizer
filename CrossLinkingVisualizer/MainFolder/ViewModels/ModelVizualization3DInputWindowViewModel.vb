Imports System.ComponentModel
Imports VBExtensions.IOExtensions

Public Class ModelVizualization3DInputWindowViewModel
    Implements ComponentModel.INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property VisualizationViewModels As ObjectModel.ObservableCollection(Of Crosslinking3DVisualizationViewModel)

    Public Property SelectedSourceObject As Crosslinking3DVisualizationViewModel
    Public Property SelectedFileName As Crosslinking3DVisualizationViewModel

    Public ReadOnly Property Command_AddFilesToCurrentObject As ICommand
    Public ReadOnly Property Command_RemoveSelectedFile As ICommand

    Public Sub New()

    End Sub

    Public Sub New(
                   inputSourceObjects As List(Of CrosslinkingSourceObjectViewModel),
                   inputModels As List(Of Crosslinking3DVisualizationViewModel)
                   )

        'grab the existing models
        VisualizationViewModels = New ObjectModel.ObservableCollection(Of Crosslinking3DVisualizationViewModel)(inputModels)

        'add new ones for objects without an existing model
        For Each sourceObject In inputSourceObjects.Except(inputModels.Select(Function(x) x.SourceCrosslinkingObject))
            VisualizationViewModels.Add(New Crosslinking3DVisualizationViewModel(sourceObject))
        Next

        AddHandler VisualizationViewModels.CollectionChanged, AddressOf OnCollectionChanged

        Command_AddFilesToCurrentObject = New DelegateCommand(Of String)(AddressOf AddFilesToCurrentObject)
        Command_RemoveSelectedFile = New DelegateCommand(Of String)(AddressOf RemoveSelectedFile)



    End Sub

    Private Sub RemoveSelectedFile(obj As String)

        With SelectedSourceObject.MolecularModels
            .Remove(.Where(Function(x) String.Equals(x.Filename.FilePath, SelectedFileName)).Single())
        End With

    End Sub

    Private Sub AddFilesToCurrentObject(obj As String)

        Dim InputFiles As List(Of String) = FileImporter.SelectFiles().Select(Function(x) x.FilePath).ToList()

        For Each file In InputFiles.Except(SelectedSourceObject.MolecularModels.Select(Function(x) x.Filename.FilePath))

            Dim newMolecularModel As New MolecularModelViewModel(New ExistingFilePath(file))
            SelectedSourceObject.MolecularModels.Add(newMolecularModel)

        Next

    End Sub



    Private Sub OnCollectionChanged(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(VisualizationViewModels)))
    End Sub
End Class
