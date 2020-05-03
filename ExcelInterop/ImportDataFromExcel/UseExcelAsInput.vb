Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Collections.Generic
Imports System.Threading.Tasks

Public Partial Module ExtendedFunctions
    
    <ComponentModel.Description("Opens Excel with a pre-set table and lets you fill it out")>
    Public async Function UseExcelAsInput(InputHeaders As List(Of String)) As Task(Of Object(,))
        
        If InputHeaders Is Nothing Then
            Throw New ArgumentNullException()
        End If

        If InputHeaders.Count = 0 then
            Throw new ArgumentException("Headers list must have at least one element")
        End If


        Dim FinalValue as Object(,)

        Try 
            FinalValue = Await UseExcelAsInputCOMInterop(InputHeaders)
        Catch ex As Exception
            Throw New Exception("COM portion of workbook data grab failed")
        Finally
            
        End Try

        If FinalValue.GetLength(1) <> InputHeaders.Count then
            Throw New Exception("Input failed because Excel returned a wrong number of columns")
        ENd If

        For i=1 To InputHeaders.Count
            If FinalValue(1,i).ToString() <> InputHeaders.Item(i-1) Then
                Throw New Exception("Input failed because the returned headers don't match the input headers")
            End If
        Next

        Return TrimEmptyRowsOrColumns(FinalValue,TrimDirection.Bottom)
        
    End Function
    
    Private Async Function UseExcelAsInputCOMInterop(InputHeaders As List(Of String)) As Task(Of Object(,))
        
        Const DoMarshalReleaseObjects As Boolean = True

        'used to wait until the workbook is closed to continue execution
        Dim WorkbookSelectionTask as new System.Threading.Tasks.TaskCompletionSource(Of Boolean)
        Dim finalValues As Object(,)
        
        Dim ExcelApplication As Excel.Application
        Dim wb As Excel.Workbook
        dim ws as Excel.Worksheet
        Dim InputTable as Excel.ListObject
        
        ExcelApplication = GetExcelInstanceCOMInterop()
        wb = GetWorkbookInstanceComInterop(ExcelApplication)

        'get the worksheet
        Try
            dim wsheets As Excel.Sheets
            wsheets = wb.Worksheets

            If wsheets.Count>0 Then
                ws = CType(wsheets(1),Excel.Worksheet)
            Else 
                ws = CType(wsheets.Add(Type:=GetType(Excel.Worksheet)),Excel.Worksheet)
            end If

            if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(wsheets)

        Catch ex As Exception
            Throw New Exception("could not create a sheet to use", ex)
        End try
                
        'create the actual table with the required header
        Try 

            Dim StartCell As Excel.Range = CType(ws.Cells(1,1),Excel.Range)
            Dim EndCell As Excel.Range = CType(ws.Cells(1,InputHeaders.Count),Excel.Range)
            Dim HeaderRange as Excel.Range = ws.Range(StartCell, EndCell)

            if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(StartCell)
            if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(EndCell)

            HeaderRange.Value2=ListToExcelCompatibleArray(InputHeaders)
            Dim TableRange as Excel.Range = HeaderRange.Resize(6)

            if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(HeaderRange)
                    
            Dim lo as Excel.ListObjects = ws.ListObjects
            InputTable = lo.Add(
                        SourceType := Excel.XlListObjectSourceType.xlSrcRange,
                        Source:=TableRange,
                        XlListObjectHasHeaders := Excel.XlYesNoGuess.xlYes)
                    
            if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(TableRange)
            
        Catch ex As Exception
            Throw New Exception("Failed setting up the table", ex)
        End Try

        ExcelApplication.Visible = True
        'TODO: grab focus!

        Dim ExitHandler as Excel.WorkbookEvents_BeforeCloseEventHandler = Sub(ByRef Cancel as Boolean)
                Try

                    Dim rs as Excel.Range = InputTable.Range

                    if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(InputTable)
                    
                    'when only one cell is selected, special handling is necessary
                    If rs.Count > 1 Then
                        finalValues = Ctype(rs.Value2,Object(,))
                    else
                        finalValues =CType(Array.CreateInstance(Gettype(Object), lengths:={1,1}, lowerbounds:={1,1}), Object(,))
                        finalvalues(1,1) = rs.Value2
                    End If

                    if DoMarshalReleaseObjects Then Runtime.InteropServices.Marshal.ReleaseComObject(rs)
                    
                Catch ex As Exception
                    Throw New Exception("Could not select data", ex)
                Finally
                    
                    'prevents asking the user whether they want to save the workbook
                    wb.Saved=True

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