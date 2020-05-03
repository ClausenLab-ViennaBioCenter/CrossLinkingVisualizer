Public Class RangeFilter
    Implements IValueFilter

    Private _RangeLowerFilter As Double
    Private _RangeUpperFilter As Double
    Public Event PropertyChanged As ComponentModel.PropertyChangedEventHandler Implements IValueFilter.PropertyChanged

    Public ReadOnly Property ReferenceObjectViewModel() As CrosslinkingSourceObjectViewModel

    Public Property RangeLowerFilter As Double Implements IValueFilter.LowLimit
        Get
            Return _RangeLowerFilter
        End Get
        Set
            _RangeLowerFilter = Value
            RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs("RangeLowerFilter"))
        End Set
    End Property

    Public Property RangeUpperFilter As Double Implements IValueFilter.HighLimit
        Get
            Return _RangeUpperFilter
        End Get
        Set
            _RangeUpperFilter = Value
            RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs("RangeUpperFilter"))
        End Set
    End Property

    Public Sub New(crosslinkingSourceObjectViewModel As CrosslinkingSourceObjectViewModel)
        ReferenceObjectViewModel = crosslinkingSourceObjectViewModel
        RangeLowerFilter = ReferenceObjectViewModel.StartIndex
        RangeUpperFilter = ReferenceObjectViewModel.EndIndex
    End Sub

    Private Function PassesSourceFilter(crosslinkViewModel As CrosslinkViewModel) As Boolean

        With crosslinkViewModel
            Return .SourceLocus >= RangeLowerFilter AndAlso .SourceLocus <= RangeUpperFilter
        End With

    End Function

    Private Function PassesTargetFilter(crosslinkViewModel As CrosslinkViewModel) As Boolean

        With crosslinkViewModel
            Return .TargetLocus >= RangeLowerFilter AndAlso .TargetLocus <= RangeUpperFilter
        End With

    End Function

    Public Function CheckFilter(crosslinkViewModel As CrosslinkViewModel) As Boolean Implements IValueFilter.CheckFilter

        With crosslinkViewModel

            If .SourceObjectViewModel Is ReferenceObjectViewModel Then

                If .TargetObjectViewModel Is ReferenceObjectViewModel Then
                    Return PassesSourceFilter(crosslinkViewModel) OrElse PassesTargetFilter(crosslinkViewModel)
                Else
                    Return PassesSourceFilter(crosslinkViewModel)
                End If

            Else

                Return PassesTargetFilter(crosslinkViewModel)

            End If

        End With

    End Function


End Class
