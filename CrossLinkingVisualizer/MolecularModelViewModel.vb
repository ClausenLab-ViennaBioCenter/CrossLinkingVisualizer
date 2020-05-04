Imports System.ComponentModel
Imports VBExtensions

Public Class MolecularModelViewModel
    Implements ComponentModel.INotifyPropertyChanged

    Private _ModelName As String
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property Filename As ExistingFilePath

    Public Property ModelName() As String
        Get
            Return _ModelName
        End Get
        Set
            _ModelName = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ModelName)))
        End Set
    End Property

    Public Sub New()

    End Sub

    Public Sub New(iFilename As ExistingFilePath)

        Filename = iFilename
        ModelName = IO.Path.GetFileNameWithoutExtension(iFilename.FilePath)

    End Sub

End Class
