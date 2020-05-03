Imports System.Collections.Generic
Imports Microsoft.Office.Interop

'Friend means it will be available in the current assembly (and each project generally makes its own assembly)
'Attribute ensures that the unit test for this assembly can see its internal stuff
<assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ExcelInteropUnitTesting")>

Public Module PublicConstants
    Public Const ExcelColumnRank As Integer = 2
    Public Const ExcelRowRank As Integer = 1
End Module

Friend Module InternalMethods
    
    Friend Const DoMarshalReleaseObjects as Boolean = True

    Friend Function WorkbookValid(Path as String) As Boolean
        Return True
    End Function
    
    Friend Sub COMRelease(TargetCOMObject as Object)
        If DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(TargetCOMObject)
    End Sub

    Friend Function ListToExcelCompatibleArray(InputList As List(Of String)) As Object(,)

        Dim FinalObject as Object(,) = CType(Array.CreateInstance(GetType(Object),{1,InputList.Count},{1,1}),Object(,))

        For i = 1 To InputList.Count
            FinalObject(1,i) = InputList.Item(i-1)
        Next

        Return FinalObject

    End Function

    Friend Function GetExcelInstanceCOMInterop() As Excel.Application

        Dim ExcelApplication As Excel.Application

        Try
            'grab existing Excel process
            ExcelApplication = CType(Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application"), Excel.Application)

        Catch ex As System.Runtime.InteropServices.COMException

            'no Excel process instantiated yet, instantiate a new one
            ExcelApplication = new Excel.Application()

        Catch ex As  Exception

            Throw new Exception("Could not start Excel",ex)

        End Try

        Return ExcelApplication

    End Function

    Friend Function GetWorkbookInstanceComInterop(ExcelApplication As Excel.Application) As Excel.Workbook
        
        Dim wb as Excel.Workbook

        'get the workbook
        Try
            Dim wbs As Excel.Workbooks = ExcelApplication.Workbooks
            wb = wbs.Add()
            COMRelease(wbs)
        Catch ex As Exception
            Throw New Exception("Could not create workbook",ex)
        End try

        Return wb

    End Function

End Module
