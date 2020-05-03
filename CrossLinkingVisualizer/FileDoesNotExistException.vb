Imports System.Runtime.Serialization


<Serializable>
    Public Class FileDoesNotExistException
        Inherits ArgumentException

        Public Sub New()
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

        Public Sub New(message As String, paramName As String)
            MyBase.New(message, paramName)
        End Sub

        Public Sub New(message As String, paramName As String, innerException As Exception)
            MyBase.New(message, paramName, innerException)
        End Sub

        Protected Sub New(info As SerializationInfo, context As StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class

