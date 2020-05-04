Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports Microsoft.VisualBasic.CompilerServices

Public Class PymolController
    Implements IDisposable

    Public ReadOnly Property Process As Process
    Public ReadOnly Property InputStream as StreamWriter

    Sub New()

        Dim pymolPath As String = My.Settings.PymolExecutablePath

        dim path as String = "C:\WINDOWS\system32\cmd.exe"
        
        Dim startInfo = new ProcessStartInfo(path)
        
        With startInfo
            .Arguments =""
            'TODO: make it hidden, but for now it's left like this since if I close the pymol window, the cmd window still stays on and I can't close it
            '.WindowStyle = ProcessWindowStyle.Hidden
            '.CreateNoWindow = True
            '.Arguments ="/k C:\Users\juraj.ahel\AppData\Local\Schrodinger\PyMOL2\PyMOLWin.exe -pK"
            '.Arguments =""
            .RedirectStandardInput = true
            '.RedirectStandardError=True
            '.RedirectStandardOutput=True
            .UseShellExecute = false
        End with

        process = new Process()
        process.StartInfo = startInfo
        process.Start()
        'process.WaitForInputIdle()
        
        Console.WriteLine("end")

        InputStream = process.StandardInput
        'dim outputReader= process.StandardOutput
        'dim errorReader= process.StandardError
        
        InputStream.WriteLine(pymolPath & " -pK")
        
        'InputStream.Flush()

        'InputStream.WriteLine("fetch 1UBQ")
        
        
        
        'Dim input as String

        'Do
        '
        '    Thread.Sleep(100)
        '    Input=Console.ReadLine()
        '
        '    If Input.ToUpper()="EXIT" Then
        '        Goto EndMe
        '    End If
        '
        '    InputStream.WriteLine(Input)
        '    InputStream.Flush()
        '    
        'Loop

        EndMe:
        
         ' process.WaitForExit()
        

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Try
            Process.close
        Catch ex As Exception

        End Try
    End Sub

    Public Shared Sub UpdatePymolPath()

        Dim ExistingPath = My.Settings.PymolExecutablePath

        Dim NewPath = InputBox("Give path to pymol.exe", "pymol.exe path", ExistingPath)

        My.Settings.PymolExecutablePath = NewPath
        My.Settings.Save()


    End Sub
End Class
