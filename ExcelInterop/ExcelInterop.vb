
Imports Excel = Microsoft.Office.Interop.Excel

Public Module ExtendedFunctions

    Public Function GrabDataFromExcelWorkbook(WorkBookPath As String, WorksheetName As String, RangeName As String) As Object(,)

        Dim App As New Excel.Application

        Dim wb As Excel.Workbook
        Dim st As Excel.Worksheet
        Dim rg1 As Excel.Range


        Dim newobj As Object(,)

        With App
            .Visible = False
            wb = .Workbooks.Open(WorkBookPath)
        End With

        st = CType(wb.Sheets(WorksheetName), Excel.Worksheet)

        rg1 = st.Range(RangeName)
        newobj = CType(rg1.Value2, Object(,))

        GrabDataFromExcelWorkbook = newobj

        GoTo CleanUpCode

CleanUpCode:
        wb.Close()
        'App.Visible = False
        'App.Visible = True

    End Function

End Module