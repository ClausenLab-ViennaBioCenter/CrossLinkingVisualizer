Public Class BindingHelper

    Public Shared Sub BindSourceToTarget(
                                         Source As Object,
                                         SourcePropertyPathString As String,
                                         Target As DependencyObject,
                                         TargetDependencyProperty As DependencyProperty,
                                         Optional TargetBindingMode As BindingMode = BindingMode.TwoWay,
                                         Optional iConverter As IValueConverter = Nothing)
        BindSourceToTarget(Source, New PropertyPath(SourcePropertyPathString), Target, TargetDependencyProperty, TargetBindingMode, iConverter)
    End Sub

    Public Shared Sub BindSourceToTarget(
                                         Source As Object,
                                         SourcePropertyPath As PropertyPath,
                                         Target As DependencyObject,
                                         TargetDependencyProperty As DependencyProperty,
                                         Optional TargetBindingMode As BindingMode = BindingMode.TwoWay,
                                         Optional iConverter As IValueConverter = Nothing)

        Dim tempBind As Binding = New Binding()

        With tempBind
            .Mode = TargetBindingMode
            .Path = SourcePropertyPath
            .Source = Source
            If iConverter IsNot Nothing Then
                .Converter = iConverter
            End If
        End With

        BindingOperations.SetBinding(Target, TargetDependencyProperty, tempBind)

    End Sub

End Class
