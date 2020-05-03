Imports System.IO


'<DebuggerStepThrough()>
Public Class ExistingFilePath
        Inherits FilePath
        Public Sub New(fileName As String)
            MyBase.New(fileName)
            If Not File.Exists(fileName) Then
                Throw New FileDoesNotExistException("Filename " & fileName & "does not exist.")
            End If
        End Sub
    End Class


