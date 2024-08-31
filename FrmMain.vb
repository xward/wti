Public Class FrmMain

    ' display layout, degiro status / led tracker, show asset prices, orders and position (merged as trade)


    ' place sell order, update order, delete order


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' init ui
        Me.Left = My.Computer.Screen.Bounds.Size.Width - Me.Width

        ' Edge init
        Edge.ensureRunning()
        Edge.printAllEdge()


        ' update ester rate
        Ester.fetchRateFromBCE()
        esterLabel.Text = "ester: " & Ester.rate

    End Sub

    Private ledBlink As Boolean = False
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles TmrUI.Tick
        If statusLabel.Text <> status.ToString Then statusLabel.Text = status.ToString

        Select Case status
            Case StatusEnum.OFFLINE
                status = StatusEnum.OFFLINE
            Case StatusEnum.ONLINE
                statusLed.BackgroundImage = PictureLedGreenOff.Image
            Case StatusEnum.SIMU, StatusEnum.LIVE
                If ledBlink Then
                    statusLed.BackgroundImage = PictureLedGreenOn.Image
                Else
                    statusLed.BackgroundImage = PictureLedGreenOff.Image
                End If
                ledBlink = Not ledBlink
        End Select


        ' LblDegiroState.Text = Degiro.degiroState.ToString

        'Label1.Text = ""
        'For Each assetName As String In TradingView.currentPrice.Keys
        '    Dim price As AssetPrice = CType(TradingView.currentPrice(assetName), AssetPrice)
        '    Label1.Text &= assetName & " " & price.dat.ToString & " " & price.price & " " & price.todayChangePerc & "%" & vbCrLf
        'Next

        Application.DoEvents()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Degiro.updateAll()

        TradingView.fetchPrice(AssetEnum.WTI3x)
        TradingView.fetchPrice(AssetEnum.WTI3xShort)
        'Degiro.setupWindows()
        'Edge.createTab("ddg.gg", Edge.OpenModeEnum.AS_WINDOW)
        'Edge.createTabIfNotExist("youtube", "https://www.youtube.com/", Edge.OpenModeEnum.AS_WINDOW, New Rectangle(3, 3, 500, 500))
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        'Edge.updateEdgeProcess()
        'dbg.info(Edge.edgeProcess.MainWindowTitle)



        MsgBox(Date.Parse("8/21/2023 16:51:21"))


        ' Degiro.checkLoggedIn()
        ' Degiro.updateOrders()
    End Sub
End Class
