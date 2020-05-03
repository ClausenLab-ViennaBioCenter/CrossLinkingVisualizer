


Public Class FileImporter

        Private Sub New

        End Sub


        Public Shared Function SelectFile(iMultiselect As Boolean) As ExistingFilePath

            Dim Dialog As New Microsoft.Win32.OpenFileDialog
            
            With Dialog

                .Multiselect = iMultiselect
                .AddExtension = False
                .CheckFileExists = True

                Dim DialogResult = .ShowDialog()

                If Not DialogResult = True Then
                    Return Nothing
                End If

                    Dim SourceFilePath as ExistingFilePath = New ExistingFilePath(.FileName)

                IdentifyFileType()

                If Not FileIsValid() Then
                    Throw New ArgumentException()
                End If

                Return SourceFilepath

            End With

            

            'SourceFilePath 
        End Function

        Public Shared Function IdentifyFileType() As String
        
        End Function

        Private Shared Function FileIsValid() As Boolean
            Return True
        End Function

    End Class
