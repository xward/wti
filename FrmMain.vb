Public Class FrmMain


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'position window
        Me.Left = My.Computer.Screen.Bounds.Size.Width - Me.Width

        ' dump all windows available at startup
        WinHandle.printProcesses()


        ' update ester rate
        '  Ester.fetchRateFromBCE()


        'Degiro.pairToEdgeDegiro()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles TmrUI.Tick
        LblDegiroState.Text = Degiro.degiroState.ToString
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Degiro.pairToEdgeDegiro()
    End Sub


    '    String link = "https://docs.microsoft.com/en-us/windows/communitytoolkit/mvvm/introduction";

    'Process? process = Process.Start(New ProcessStartInfo(link)
    '{
    '    UseShellExecute = true
    '});

    'process!.WaitForExit();
End Class
