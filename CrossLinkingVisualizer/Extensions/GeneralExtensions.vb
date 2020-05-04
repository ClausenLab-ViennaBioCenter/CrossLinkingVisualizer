Imports System.Runtime.CompilerServices

Public Module GeneralExtensions

    <Extension()>
    Public Function [In](Of TItem)(Item As TItem, ParamArray ValueList() As TItem) As Boolean
        Return Array.IndexOf(ValueList, Item) > -1
    End Function

    <Extension()>
    Public Function [In](Of TItem)(Item As TItem, ValueList As IList) As Boolean
        Return ValueList.Contains(Item)
    End Function


    '****************************************************************************************************
    Public Sub SwapValue(a As Object, b As Object)

        '====================================================================================================
        'Swaps two values of any type variable
        'Juraj Ahel, 2015-04-30, for general purposes
        'Last update 2015-04-30
        '====================================================================================================

        Dim c As Object

        c = a
        a = b
        b = c

    End Sub

    '****************************************************************************************************
    Public Sub SwapValue(Of T As Structure)(ByRef a As T, ByRef b As T)

        '====================================================================================================
        'Swaps two values of any type variable
        'Juraj Ahel, 2015-04-30, for general purposes
        'Last update 2015-04-30
        '====================================================================================================

        Dim c As T

        c = a
        a = b
        b = c

    End Sub

End Module
