Public Class ValidatedName

    Public ReadOnly Property Name() As String

    Public Shared Function IsValid(iName As String)
        Return True
    End Function

    Public Sub New(iName As String)
        If IsValid(iName) Then
            Name = iName
        Else
            Throw New ArgumentException(Format("Tried to initialize a {0} with an invalid string", "xxX"))
        End If
    End Sub



End Class