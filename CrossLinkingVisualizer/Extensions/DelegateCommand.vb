Imports System.Windows.Input

Public Class DelegateCommand(Of T As Class)
    Implements ICommand

    Private _canExecute As Predicate(Of T)
    Private _execute As Action(Of T)
    Private _exceptionHandler As Action(Of Exception)

    Public Sub New(execute As Action(Of T), exceptionHandler As Action(Of Exception))
        Me.New(execute, Nothing, exceptionHandler)
    End Sub

    Public Sub New(execute As Action(Of T))
        Me.New(execute, Nothing, Nothing)
    End Sub

    Public Sub New(execute As Action(Of T), canExecute As Predicate(Of T), exceptionHandler As Action(Of Exception))
        _execute = execute
        _canExecute = canExecute
        _exceptionHandler = exceptionHandler
    End Sub

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute

        If _CanExecute Is Nothing Then
            Return True
        Else
            Return _CanExecute(CType(parameter, T))
        End If


    End Function

    Public Sub Execute(parameter As Object) Implements ICommand.Execute

        Dim TParam As T = CType(parameter, T)

        Try
            _execute(TParam)
        Catch ex As Exception
            _exceptionHandler(ex)
        End Try

    End Sub

    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

    Public Sub RaiseCanExecuteChanged()

        RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)

    End Sub

End Class
