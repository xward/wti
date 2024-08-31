Public Class FrmMain

    ' degiro orders and positions
    ' display layout, degiro status / led tracker, show asset prices, orders and position (merged as trade)



    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' init ui
        Me.Left = My.Computer.Screen.Bounds.Size.Width - Me.Width

        ' Edge init
        Edge.ensureRunning()
        Edge.printAllEdge()


        ' update ester rate
        ' Ester.fetchRateFromBCE()


    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles TmrUI.Tick
        ' LblDegiroState.Text = Degiro.degiroState.ToString

        Label1.Text = ""
        For Each assetName As String In TradingView.currentPrice.Keys
            Dim price As AssetPrice = CType(TradingView.currentPrice(assetName), AssetPrice)
            Label1.Text &= assetName & " " & price.dat.ToString & " " & price.price & " " & price.todayChangePerc & "%" & vbCrLf
        Next
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Degiro.updateAll()

        'Degiro.setupWindows()
        'Edge.createTab("ddg.gg", Edge.OpenModeEnum.AS_WINDOW)
        'Edge.createTabIfNotExist("youtube", "https://www.youtube.com/", Edge.OpenModeEnum.AS_WINDOW, New Rectangle(3, 3, 500, 500))
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'Edge.updateEdgeProcess()
        'dbg.info(Edge.edgeProcess.MainWindowTitle)

        TradingView.fetchPrice(AssetEnum.WTI3x)
        TradingView.fetchPrice(AssetEnum.WTI3xShort)

        ' Degiro.checkLoggedIn()
        ' Degiro.updateOrders()
    End Sub
End Class
