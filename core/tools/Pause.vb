Imports System.Threading

Module TimeHelper
    Private randomO As New Random

    Public Function rand(ByVal range As Integer) As Integer
        Return CInt(Math.Floor(randomO.NextDouble() * (range + 1)))
    End Function


    Public doStopCurrentPause As Boolean = False
    Public Sub RandPause(ByVal elapseMin As Integer, ByVal elapseMax As Integer, Optional ByVal doPrintElasped As Boolean = False)
        Dim p As Integer = rand(elapseMax - elapseMin) + elapseMin
        Pause(p, doPrintElasped)
    End Sub


    Public Sub Pause(ByVal elapse As Integer, Optional ByVal doPrintElasped As Boolean = False)
        If elapse = 0 Then Exit Sub
        If elapse < 501 Then
            Thread.Sleep(elapse)
            Exit Sub
        End If

        Dim startingPause As Date = Now

        While (Now.Subtract(startingPause).TotalMilliseconds < elapse)
            Thread.Sleep(35)

            'print elasped in a label
            If doPrintElasped And Now.Subtract(startingPause).TotalSeconds > 1 Then
                '  FrmDashBoard.Label1STR = "Pause while " & Now.Subtract(startingPause).ToString
            End If

            'If FrmDashBoard.b0tRunning = False Then
            '    If Not (FrmDashBoard.tPlay Is Nothing) Then
            '        If FrmDashBoard.tPlay.IsAlive Then
            '            ' If kmOutMode = "rs232" Then UsbKMRS232.releaseAllKeys() ' @todo: close if port was opened
            '            FrmDashBoard.tPlay.Abort()
            '        End If
            '    End If
            '    Exit Sub
            'End If
            If doStopCurrentPause Then
                doStopCurrentPause = False
                Exit Sub
            End If
        End While

    End Sub

    ' ex to wait next 03h45 timeDate="XX/XX/XXXX 03:45:XX"
    Public Sub waitUntilTimeDate(ByVal timeDate As String, Optional ByVal doPrintElasped As Boolean = False)
        Dim over As Boolean = False
        Dim st As String
        Dim a, b As String

        While Not over
            Thread.Sleep(750)
            st = Now.ToString  ' 20/05/2014 13:05:12

            over = True
            For nu = 0 To st.Length - 1
                a = st.Substring(nu, 1)
                b = timeDate.Substring(nu, 1)
                If b = "X" Then Continue For
                If a <> b Then over = False
            Next

            'print elasped in a label
            'If doPrintElasped Then
            '    FrmDashBoard.Label1STR = "Pause until " & timeDate & " now is " & Now.ToString
            'End If

        End While


    End Sub

End Module

