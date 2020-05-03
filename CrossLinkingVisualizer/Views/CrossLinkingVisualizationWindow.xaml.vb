Public Class CrossLinkingVisualizationWindow
    Private Sub button_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub OnClosing(sender As Object, e As ComponentModel.CancelEventArgs)
        e.Cancel = True
        Me.Hide()
    End Sub

End Class
