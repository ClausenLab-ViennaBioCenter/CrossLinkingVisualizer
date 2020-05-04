Public Class RangeSlider

    #Region "Dependency"
    Public Property Minimum as Double
    Get
        Return CDbl(GetValue(MinimumProperty))
    End Get
        Set(value as Double)
            SetValue(MinimumProperty, value)
        End Set
    End Property

    Public Property Maximum as Double
        Get
            Return CDbl(GetValue(MaximumProperty))
        End Get
        Set(value as Double)
            SetValue(MaximumProperty, value)
        End Set
    End Property

    Public Property LowerValue as Double
        Get
            Return CDbl(GetValue(LowerValueProperty))
        End Get
        Set(value as Double)
            SetValue(LowerValueProperty, value)
        End Set
    End Property

    Public Property UpperValue as Double
        Get
            Return CDbl(GetValue(UpperValueProperty))
        End Get
        Set(value as Double)
            SetValue(UpperValueProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MinimumProperty As DependencyProperty = DependencyProperty.Register("Minimum", GetType(Double), GetType(RangeSlider), new UIPropertyMetadata(CDbl(0)))
    Public Shared ReadOnly MaximumProperty As DependencyProperty = DependencyProperty.Register("Maximum", GetType(Double), GetType(RangeSlider), new UIPropertyMetadata(CDbl(1)))
    Public Shared ReadOnly LowerValueProperty As DependencyProperty = DependencyProperty.Register("LowerValue", GetType(Double), GetType(RangeSlider), new UIPropertyMetadata(CDbl(0)))
    Public Shared ReadOnly UpperValueProperty As DependencyProperty = DependencyProperty.Register("UpperValue", GetType(Double), GetType(RangeSlider), new UIPropertyMetadata(CDbl(1)))

    #End Region

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        AddHandler Loaded, AddressOf OnControlLoaded

    End Sub


    Private Sub OnControlLoaded(sender As Object, e As RoutedEventArgs)

        AddHandler LowerSlider.ValueChanged, AddressOf OnLowerValueChange
        AddHandler UpperSlider.ValueChanged, AddressOf OnUpperValueChange

    End Sub

    Private Sub OnLowerValueChange(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        UpperSlider.Value = Math.Max(LowerSlider.Value,UpperSlider.Value)
    End Sub

    Private Sub OnUpperValueChange(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        LowerSlider.Value = Math.Min(LowerSlider.Value,UpperSlider.Value)
        End Sub



End Class
