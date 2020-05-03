Imports Microsoft.Office.Interop
Imports System.Threading.Tasks

Public Partial Module ExtendedFunctions

    <ComponentModel.Description("Opens the target excel table and grabs the values in selected range at exit")>
    Public async Function SelectFromExcelTable(WorkBookPath As String) As Task(Of Object(,))
        
        Const AllowTempFileOverwrite as Boolean = True

        If Not WorkbookValid(WorkBookPath) Then
            Throw New ArgumentException("No valid workbook found at given path")
        End If

        Dim TargetPathTemp As String = 
                IO.Path.GetTempPath() & 
                IO.Path.GetFileNameWithoutExtension(WorkBookPath) &
                "_temp" &
                IO.Path.GetExtension(WorkBookPath)
        
        Try
            IO.File.Copy(WorkBookPath, TargetPathTemp, AllowTempFileOverwrite)
        Catch ex As Exception
            Throw New Exception("Could not create a temp working copy", ex)
        End Try
        
        Dim FinalValue as Object(,)

        Try 
            FinalValue = Await SelectFromExcelTableCOMInterop(TargetPathTemp)
        Catch ex As Exception
            Throw New Exception("COM portion of workbook data grab failed")
        Finally
            Try
                IO.File.Delete(TargetPathTemp)
            Catch ex As Exception
                Diagnostics.Debug.Print("Exception encountered when trying to delete the temp file")
            End Try
            'Cleanup of COM objects needs to be done this way - but it works only in release
            'not really in debug, according to stackoverflow
            'Manually trying to marshal them away is NOT the way to do it
            '...except, the GC didn't work. Marshalling did. So it stays for now.
            'Dim GCCounter as Integer = 0
            'Do While System.Runtime.InteropServices.Marshal.AreComObjectsAvailableForCleanup()
            '    GC.Collect()
            '    GC.WaitForPendingFinalizers()
            '    GCCounter+=1
            'Loop
            'MsgBox(String.Format("GC ran {0} times", GCCounter))
        End Try

        Return FinalValue
        
    End Function

    Private Async Function SelectFromExcelTableCOMInterop(WorkBookPath As String) As Task(Of Object(,))
        
        Const DoMarshalReleaseObjects As Boolean = True

        'used to wait until the workbook is closed to continue execution
        Dim WorkbookSelectionTask as new System.Threading.Tasks.TaskCompletionSource(Of Boolean)
        Dim finalValues As Object(,)


        Dim ExcelApplication As Excel.Application
        Dim wb As Excel.Workbook

        Try
            ExcelApplication = CType(Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application"), Excel.Application)
        Catch ex As System.Runtime.InteropServices.COMException
            ExcelApplication = new Excel.Application()
        Catch ex As  Exception
            Throw new Exception("Could not start Excel",ex)
        End Try
        
        With ExcelApplication
            .Visible = True
            Try
                Dim wbs As Excel.Workbooks = .Workbooks
                wb = wbs.Open(WorkBookPath)
                if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(wbs)
            Catch ex As Exception
                Throw New Exception("Could not open file",ex)
            End try
            'MsgBox("workbook opened")
        End With
        
        Dim ExitHandler as Excel.WorkbookEvents_BeforeCloseEventHandler = Sub(ByRef Cancel as Boolean)
                Try
                    If Cancel then exit sub
                    Dim aw as Excel.Window = ExcelApplication.ActiveWindow
                    Dim rs as Excel.Range = aw.RangeSelection
                    'when only one cell is selected, 
                    If rs.Count > 1 Then
                        finalValues = Ctype(rs.Value2,Object(,))
                    else
                        finalValues =CType(Array.CreateInstance(Gettype(Object), lengths:={1,1}, lowerbounds:={1,1}), Object(,))
                        finalvalues(1,1) = rs.Value2
                    End If
                    if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(rs)
                    if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(aw)
                Catch ex As Exception
                    Throw New Exception("Could not select data", ex)
                Finally
                    if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(wb)
                    if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(ExcelApplication)
                End Try
                WorkbookSelectionTask.SetResult(True)
            End Sub

        'AddHandler ExcelApplication.WorkbookBeforeClose, ExitHandler
        AddHandler wb.BeforeClose, ExitHandler
        Await WorkbookSelectionTask.Task
        RemoveHandler wb.BeforeClose, ExitHandler

        Return finalValues

    End Function

    
End Module
