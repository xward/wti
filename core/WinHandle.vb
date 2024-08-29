Imports System.DirectoryServices.ActiveDirectory
Imports System.Security.Policy

Public Module WinHandle
    Public dbg As New N3rdDebug("main", dbgIndentLevel.subMaster1)


    ' <WTI 75,122 ▼ −1.82% MSCI SP500 NASDAQ - Personal - Microsoft​ Edge>  process msedge

    Public Function listProcesses() As List(Of Process)
        Dim processes As New List(Of Process)
        For Each p As Process In Process.GetProcesses
            If p.MainWindowTitle = String.Empty = False Then processes.Add(p)
        Next
        Return processes
    End Function

    Public Sub printProcesses()
        For Each p As Process In Process.GetProcesses
            If p.MainWindowTitle = String.Empty = False Then
                dbg.info("Windows available: <" & p.MainWindowTitle & ">  process " & p.ProcessName & " (" & p.Id & ")")
            End If
        Next
    End Sub




End Module


' Dim p As Process =
'   System.Diagnostics.Process.Start("C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", "https://trader.degiro.nl/login/fr#/login")

'Pause(1000)
'Dim Processes As Process() = Process.GetProcessesByName("msedge")

'For Each p As Process In Processes

'If Not p.MainWindowTitle.Contains("Personal - Microsoft​ Edge") Then Continue For

'                dbgMain.info("edge available: <" & p.MainWindowTitle & "> process " & p.ProcessName & " (" & p.Id & ") " & User32.getWindowPos(p.MainWindowHandle).ToString)

'Next