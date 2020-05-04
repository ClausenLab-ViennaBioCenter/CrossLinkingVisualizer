


Public Class FileImporter

    Public Shared Function SelectFiles(iMultiselect As Boolean) As List(Of ExistingFilePath)

        Dim Dialog As New Microsoft.Win32.OpenFileDialog

        With Dialog

            .Multiselect = iMultiselect
            .AddExtension = False
            .CheckFileExists = True
            .CheckPathExists = True

            Dim DialogResult = .ShowDialog()

            If Not DialogResult = True Then
            End If

            Return .FileNames.Select(Function(x) New ExistingFilePath(x)).ToList()

        End With

    End Function

    Public Shared Function SelectFiles() As List(Of ExistingFilePath)

        Return SelectFiles(iMultiselect:=True)

    End Function


    Public Shared Function SelectFile() As ExistingFilePath

        Return SelectFiles(iMultiselect:=False).SingleOrDefault()

    End Function

End Class
