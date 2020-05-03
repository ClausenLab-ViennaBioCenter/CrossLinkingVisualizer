

Public Class FilePath

        Private _FilePath as String

        Public Sub New(fileName As String)
            FilePath = fileName
        End Sub

        Public Property FilePath As String
            Get
                Return _FilePath
            End Get
            Set(value As String)
                _FilePath = value
            End Set
        End Property

        Public Function ToString() As String
            Return FilePath
        End Function
        
    End Class

