Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Tab

Public Class FrmMain
    ' // show asset prices
    ' orders, position, transaction, merged as trade
    ' show trades, with current price, how far I am

    ' replay simulation, place fake order, fetch fake order/position/transaction, output results to file
    ' implem 4%/1.5% algo with

    ' place sell order, update order, delete order
    ' manage order too far from objective

    ' do regression of sp500, fetch sp500/sp5003x ratio on yahoo, produce above/below trend val/perc from live sp5003x

    ' --------------------------------------------------------------------------------------------------------

    ' si il y a du platinium, wti, copper en position ... auto-sell-stop-loss-2%, texto lors de la detection, texto lors de la vente avec resultat
    ' si qté pair prend en charge, si impair touche pas ?

    ' --------------------------------------------------------------------------------------------------------

    ' move FrmMain to frm folder
    ' split structure

    ' --------------------------------------------------------------------------------------------------------

    'set to nothing if nothing to do at start is running compiled
    Dim ACTION_AT_AUTO_START As String = "COLLECT"

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CST.init()

        Degiro.loadPastData()

        'fake data
        If Not CST.COMPILED And status = StatusEnum.OFFLINE And False Then
            Degiro.createFakeData()
        End If

        ' Edge init
        Edge.ensureRunning()
        Edge.printAllEdge()

        ' update ester rate
        Ester.fetchRateFromBCE()
        esterLabel.Text = "ester: " & Ester.rate

        initUI()

    End Sub

    Public Sub initUI()
        If CST.COMPILED Then runType.Text = "RUN" Else runType.Text = "DEBUG"

        Dim fullScreenMode As Boolean = CST.COMPILED

        ' Me
        Me.Top = 0
        If fullScreenMode Then Me.Left = Edge.edgeWindowRect.Width - 15 Else Me.Left = CST.SCREEN.Width - Me.Width
        If fullScreenMode Then Me.Width = CST.SCREEN.Width - Me.Left
        If fullScreenMode Then Me.Height = CST.SCREEN.Height

        ' auto start
        If IsNothing(ACTION_AT_AUTO_START) Or Not CST.COMPILED Then
            ToolStripStatusSays.Text = ""
        Else
            statusLed.BackgroundImage = PictureLedGreenOn.Image
            ToolStripStatusSays.Text = "About to auto-start with action " & ACTION_AT_AUTO_START
        End If

        Degiro.updateTradePanelUI()
    End Sub

    Private Sub TmerKeyIput_Tick(sender As Object, e As EventArgs) Handles TmerKeyIput.Tick
        If GetAsyncKeyState(Keys.F2) And My.Computer.Keyboard.CtrlKeyDown Then
            status = StatusEnum.COLLECT
        End If
        If GetAsyncKeyState(Keys.F3) And My.Computer.Keyboard.CtrlKeyDown Then
            status = StatusEnum.OFFLINE
            'prevent auto start at boot
            TmerAutoStart.Enabled = False
        End If
    End Sub

    Private Sub TmerAutoStart_Tick(sender As Object, e As EventArgs) Handles TmerAutoStart.Tick
        If CST.COMPILED Then status = StatusEnum.COLLECT
        TmerAutoStart.Enabled = False
    End Sub

    Dim lastCollect As Date = Date.UtcNow

    Private ledBlink As Boolean = False

    Dim assetsToTrack As New List(Of AssetInfos) From {
        assetInfo("3USL")
    }

    Private Sub TmrUI_Tick(sender As Object, e As EventArgs) Handles TmrUI.Tick
        If statusLabel.Text <> status.ToString Then statusLabel.Text = status.ToString

        Select Case status
            Case StatusEnum.OFFLINE
                status = StatusEnum.OFFLINE
                statusLed.BackgroundImage = PictureLedRedOff.Image
            Case StatusEnum.ONLINE
                statusLed.BackgroundImage = PictureLedGreenOff.Image
            Case StatusEnum.SIMU, StatusEnum.LIVE, StatusEnum.COLLECT
                If ledBlink Then
                    statusLed.BackgroundImage = PictureLedGreenOn.Image
                Else
                    statusLed.BackgroundImage = PictureLedGreenOff.Image
                End If
                ledBlink = Not ledBlink
        End Select


        If status = StatusEnum.COLLECT Then
            Dim diff As Integer = Math.Round(Date.UtcNow.Subtract(lastCollect).TotalSeconds)

            Label1.Text = "next price update " & (5 - diff) & " secs"

            If diff >= 5 And False Then
                TmrUI.Enabled = False
                Label1.Text = "updating ..."
                dbg.info("updating prices from trading view ...")
                fetchPrice(assetsToTrack)
                lastCollect = Date.UtcNow
                TmrUI.Enabled = True
            End If
        End If


        ' LblDegiroState.Text = Degiro.degiroState.ToString

        'Label1.Text = ""
        'For Each assetName As String In TradingView.currentPrice.Keys
        '    Dim price As AssetPrice = CType(TradingView.currentPrice(assetName), AssetPrice)
        '    Label1.Text &= assetName & " " & price.dat.ToString & " " & price.price & " " & price.todayChangePerc & "%" & vbCrLf
        'Next

        Application.DoEvents()
    End Sub


    ' ------------------------------------------------------------------------------------------------------
    ' Utils
    <DllImport("user32.dll")>
    Public Shared Function GetAsyncKeyState(ByVal vKey As System.Windows.Forms.Keys) As Short
    End Function


    Private Sub TestMeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TestMeToolStripMenuItem.Click
        fetchPrice(assetsToTrack)
    End Sub


    Private Sub DegiroScanToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DegiroScanToolStripMenuItem.Click
        'Edge.switchTab(Edge.TabEnum.DEGIRO_TRANSACTIONS)
        'Dim body As String = KMOut.selectAllCopy()

        'Degiro.updateTransactions(body)


        'Degiro.updateAccountDataFromBody(body)
        'Degiro.updatePositions(body)


        Degiro.updateAll()
        Degiro.updateTradePanelUI()
    End Sub
End Class
