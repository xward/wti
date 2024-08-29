Public Class FrmMain


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'position window
        Me.Left = My.Computer.Screen.Bounds.Size.Width - Me.Width

        ' dump all windows available at startup
        ' WinHandle.printProcesses()

        Edge.ensureRunning()


        Edge.printAllEdge()

        ' update ester rate
        '  Ester.fetchRateFromBCE()


        'Degiro.pairToEdgeDegiro()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles TmrUI.Tick
        ' LblDegiroState.Text = Degiro.degiroState.ToString
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Degiro.setupWindows()
        'Edge.createTab("ddg.gg", Edge.OpenModeEnum.AS_WINDOW)
        Edge.createTabIfNotExist("youtube", "https://www.youtube.com/", Edge.OpenModeEnum.AS_WINDOW, New Rectangle(3, 3, 500, 500))
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'Edge.updateEdgeProcess()
        'dbg.info(Edge.edgeProcess.MainWindowTitle)

        Degiro.checkLoggedIn()
    End Sub


    '    String link = "https://docs.microsoft.com/en-us/windows/communitytoolkit/mvvm/introduction";

    'Process? process = Process.Start(New ProcessStartInfo(link)
    '{
    '    UseShellExecute = true
    '});

    'process!.WaitForExit();
End Class
