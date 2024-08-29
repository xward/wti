Imports System.DirectoryServices.ActiveDirectory

Public Module WinHandle
    Public dbgMain As New N3rdDebug("main", dbgIndentLevel.subMaster1)


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
                dbgMain.info("Windows available: <" & p.MainWindowTitle & ">  process " & p.ProcessName & " (" & p.Id & ")")
            End If
        Next
    End Sub

    Public Function findEdgeContainingName(name As String) As Process
        For Each p As Process In Process.GetProcesses
            If p.MainWindowTitle = String.Empty = False And p.ProcessName = "msedge" And p.MainWindowTitle.Contains(name) Then Return p
        Next
        Return Nothing
    End Function

    Public Function findEdgeContainingOneNames(names As List(Of String)) As Process
        For Each p As Process In Process.GetProcesses
            If p.MainWindowTitle = String.Empty = False And p.ProcessName = "msedge" Then
                For Each name As String In names
                    If p.MainWindowTitle.Contains(name) Then Return p
                Next
            End If
        Next
        Return Nothing
    End Function

    Public Sub initEdgeDegiro()
        ' find
    End Sub


    Public Sub coucou()

    End Sub


End Module