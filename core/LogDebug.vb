Imports System.Threading


Public Module Log

  Private Const NB_LINE As Integer = 300

  Private logsStash As New ArrayList
  Public Function getLastLineOfLogs(Optional ByVal giveToMeForward As Boolean = False) As String
    Dim retStr As String = ""
    If giveToMeForward Then
      For Each s As String In logsStash
        retStr &= s & vbCrLf
      Next
    Else
      For i = logsStash.Count - 1 To 0 Step -1
        retStr &= logsStash(i) & vbCrLf
      Next
    End If
    Return retStr
  End Function

  Public Sub newLogLine(ByVal msg As String)
    Debug.WriteLine(msg)
    If logsStash.Count > NB_LINE Then logsStash.RemoveAt(0)
    logsStash.Add(msg)
  End Sub

End Module

Public Class N3rdDebug

  Private name As String
  Private indentStr As String
  Public debugLevel As Integer


  Public Sub New(ByVal name As String, ByVal indentLevel As dbgIndentLevel, Optional ByVal debugLvl As Integer = 0)
    Me.name = name

    indentStr = ""
    For nu = 0 To indentLevel
      indentStr &= " "
    Next

    debugLevel = debugLvl
  End Sub


  Public Sub dbg(ByVal msg As String)
    If Me.debugLevel > 0 Then Exit Sub
    newLogLine(indentStr & "@[" & dateStrTimestamp(Now) & _
                    "] [" & Me.name & " (Tid=" & Thread.CurrentThread.ManagedThreadId & ")]: " & msg)
  End Sub


  Public Sub info(ByVal msg As String)
    If Me.debugLevel > 0 Then Exit Sub
    newLogLine(indentStr & "@[" & dateStrTimestamp(Now) & _
                    "] [" & Me.name & " (Tid=" & Thread.CurrentThread.ManagedThreadId & ")]: " & msg)

  End Sub



  Public Sub print(ByVal msg As String)
    If Me.debugLevel > 1 Then Exit Sub
    newLogLine(indentStr & "@[" & dateStrTimestamp(Now) & _
                    "] [" & Me.name & " (Tid=" & Thread.CurrentThread.ManagedThreadId & ")]: " & msg)
  End Sub




    Public Sub fail(ByVal msg As String)

        'Console.ForegroundColor = ConsoleColor.Red
        newLogLine(indentStr & "@[" & dateStrTimestamp(Now) &
                    "] [" & Me.name & " (Tid=" & Thread.CurrentThread.ManagedThreadId & ")] ERROR " & msg)

        'Console.ResetColor()
    End Sub


    Public Function dateStrTimestamp(ByVal dat As Date) As String ' like 123d12h03m12.3245s
        Return dat.DayOfYear & "d" & dat.ToString("HH\hMM\mss.ffff\s")
    End Function


End Class


Public Enum dbgIndentLevel
  master0
  subMaster1
  truc2
  truc3
End Enum