Public Module Array2DFunctions
    
    Public Enum TrimDirection
        All
        Left
        Right
        Top
        Bottom
        LeftTop
        LeftBottom
        RightTop
        RightBottom
        LeftRight
        LeftRightTop
        LeftRightBottom
        TopBottom
        TopBottomLeft
        TopBottomRight
    End Enum

    Public Function TrimEmptyRowsOrColumns(InputArray As Object(,), Optional iTrimDirection As TrimDirection = TrimDirection.All) As Object(,)
        Return TrimEmptyRowsOrColumns(Of Object)(InputArray, iTrimDirection)
    End Function
    
    Public Function TrimEmptyRowsOrColumns(InputArray As String(,), Optional iTrimDirection As TrimDirection = TrimDirection.All) As String(,)
        Return TrimEmptyRowsOrColumns(Of String)(InputArray, iTrimDirection)
    End Function

    Public Function TrimEmptyRowsOrColumns(InputArray As Double(,), Optional iTrimDirection As TrimDirection = TrimDirection.All) As Double(,)
        Return TrimEmptyRowsOrColumns(Of Double)(InputArray, iTrimDirection)
    End Function

    Public Function TrimEmptyRowsOrColumns(InputArray As Integer(,), Optional iTrimDirection As TrimDirection = TrimDirection.All) As Integer(,)
        Return TrimEmptyRowsOrColumns(Of Integer)(InputArray, iTrimDirection)
    End Function

    Public Function TrimEmptyRowsOrColumns(Of T)(InputArray As T(,), Optional iTrimDirection As TrimDirection = TrimDirection.All) As T(,)

        If InputArray Is Nothing Then Return Nothing

        Dim TrimLeft,TrimRight,TrimTop,TrimBottom As Boolean
        Dim LeftEmpty, RightEmpty, TopEmpty, BottomEmpty As Integer

        Select Case iTrimDirection
            Case TrimDirection.All
                TrimLeft=True
                TrimRight=True
                TrimBottom=True
                TrimTop=True
            Case TrimDirection.Left
                TrimLeft=True
            Case TrimDirection.Right
                TrimRight=True
            Case TrimDirection.Top
                TrimTop=True
            Case TrimDirection.Bottom
                TrimBottom=True
            Case TrimDirection.LeftTop
                TrimLeft=True
                TrimTop=True
            Case TrimDirection.LeftBottom
                TrimLeft=True
                TrimBottom=True
            Case TrimDirection.RightTop
                TrimRight=True
                TrimTop=True
            Case TrimDirection.RightBottom
                TrimRight=True
                TrimBottom=True
            Case TrimDirection.LeftRight
                TrimLeft = True
                TrimRight = True
            Case TrimDirection.LeftRightTop
                TrimLeft = True
                TrimRight = True
                TrimTop =True
            Case TrimDirection.LeftRightBottom
                TrimLeft = True
                TrimRight = True
                TrimBottom = True
            Case TrimDirection.TopBottom
                TrimBottom = True
                TrimTop = True
            Case TrimDirection.TopBottomLeft
                TrimBottom = True
                TrimTop = True
                TrimLeft = True
            Case TrimDirection.TopBottomRight
                TrimBottom = True
                TrimTop = True
                TrimRight = True
            Case Else
                Throw New Exception()
        End Select

        Dim i,j as Integer
        
        If Not TrimLeft then goto FinalizeLeft
        For i = 1 To InputArray.GetLength(1)
            For j = 1 To InputArray.GetLength(0)
                If InputArray(j,i) IsNot Nothing Then Goto FinalizeLeft
            Next
            LeftEmpty+=1
        Next
        FinalizeLeft:

        If Not TrimRight then goto FinalizeRight
        For i = InputArray.GetLength(1) To 1 Step -1
            For j = 1 To InputArray.GetLength(0)
                If InputArray(j,i) IsNot Nothing Then Goto FinalizeRight
            Next
            RightEmpty+=1
        Next
        FinalizeRight:

        If Not TrimTop then goto FinalizeTop
        For j = 1 To InputArray.GetLength(0)
            For i = 1 To InputArray.GetLength(1)
                If InputArray(j,i) IsNot Nothing Then Goto FinalizeTop
            Next
            TopEmpty+=1
        Next
        FinalizeTop:

        If Not TrimBottom then goto FinalizeBottom
        For j = InputArray.GetLength(0) To 1 Step -1
            For i = 1 To InputArray.GetLength(1)
                If InputArray(j,i) IsNot Nothing Then Goto FinalizeBottom
            Next
            BottomEmpty+=1
        Next
        FinalizeBottom:
        
        'create a new final array with same lower bounds, but shorter and narrower
        Dim FinalArray As T(,) = CType(
                                        Array.CreateInstance(
                                            GetType(T), 
                                            {(InputArray.GetLength(0) - TopEmpty - BottomEmpty), (InputArray.GetLength(1) - LeftEmpty - RightEmpty)}, 
                                            {InputArray.GetLowerBound(0), InputArray.GetLowerBound(1)}), 
                                        T(,)
                                      )

        With FinalArray
            For i = .GetLowerBound(0) To .GetLowerBound(0) + .GetLength(0) - 1
                For j = .GetLowerBound(1) To .GetLowerBound(1) + .GetLength(1) - 1
                    FinalArray(i,j)=InputArray(TopEmpty+i,LeftEmpty+j)
                Next
            Next
        End with

        Return FinalArray
        
    End Function

    Public Function InstantiateNulls(InputArray As Object(,)) As Object(,)
        Return InstantiateNulls(Of Object)(InputArray)
    End Function

    Public Function InstantiateNulls(Of T As New)(InputArray As T(,)) As T(,)

        If InputArray Is Nothing Then Return Nothing
        
        Dim OutputArray As T(,) = InputArray

        Dim i,j as Integer
        
        For i = 1 To OutputArray.GetLength(0)
            For j = 1 To OutputArray.GetLength(1)
                If OutputArray(i,j) Is Nothing Then OutputArray(i,j) = New T
            Next
        Next

        Return OutputArray

    End Function
        
End Module
