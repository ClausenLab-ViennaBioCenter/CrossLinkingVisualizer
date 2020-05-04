Public Class ModelVizualization3DInputWindow

    <DebuggerStepThrough()>
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(inputVM As ModelVizualization3DInputWindowViewModel)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        DataContext = inputVM

    End Sub

End Class
