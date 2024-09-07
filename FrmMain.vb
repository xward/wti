Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Tab
Imports WorstTradingInitiative.CST.CST

Public Class FrmMain
    ' // show asset prices
    ' // orders, position, transaction, merged as trade
    ' // show trades, with current price, how far I am

    ' // replay simulation, place fake order, fetch fake order/position/transaction, output results to file
    ' live visual graphs 3mo+5d
    ' analyze pattern tools (chute, stable ...)
    ' implem 4%/1.5% algo with, simulate stuffs

    ' place sell order, update order, delete order
    ' manage order too far from objective

    ' do regression of sp500, fetch sp500/sp5003x ratio on yahoo, produce above/below trend val/perc from live sp5003x

    ' --------------------------------------------------------------------------------------------------------

    ' si il y a du platinium, wti, copper en position ... auto-sell-stop-loss-2%, texto lors de la detection, texto lors de la vente avec resultat
    ' si qté pair prend en charge, si impair touche pas ?

    ' --------------------------------------------------------------------------------------------------------
    ' refacto
    ' split structure
    ' move struct as class
    ' harmonixe assetInfo vs ticker string as fct inputs
    ' move imgages to ressource explorer

    ' --------------------------------------------------------------------------------------------------------

    Dim CommandLineArgs As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CST.init()

        Degiro.loadPastData()

        'fake data for display debugging
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


    Public bottomGraph As New Graph


    Public Sub initUI()
        If CST.COMPILED Then runType.Text = "RUN" Else runType.Text = "DEBUG"

        Dim fullScreenMode As Boolean = CST.COMPILED

        Me.Width = Math.Min(Me.Width, CST.SCREEN.Width - Edge.edgeWindowRect.Width)

        ' Me
        Me.Top = 0
        If fullScreenMode Then Me.Left = Edge.edgeWindowRect.Width - 15 Else Me.Left = CST.SCREEN.Width - Me.Width
        If fullScreenMode Then Me.Width = CST.SCREEN.Width - Me.Left
        Me.Height = CST.SCREEN.Height


        PanelGraphTop.Height = TopPanel.Height / 2

        ' auto start configuration
        If CST.COMPILED And CommandLineArgs.Count > 0 AndAlso CommandLineArgs(0) = "COLLECT" Then
            statusLed.BackgroundImage = PictureLedGreenOn.Image
            ToolStripStatusSays.Text = "About to auto-start with action COLLECT"
            TmerAutoStart.Enabled = True
        End If

        bottomGraph.init(PanelGraphBottom, sp5003x)

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
            ToolStripStatusSays.Text = "Interrupt by user"
        End If
    End Sub

    Private Sub TmerAutoStart_Tick(sender As Object, e As EventArgs) Handles TmerAutoStart.Tick
        ToolStripStatusSays.Text = "COLLECT begins"
        If CST.COMPILED Then status = StatusEnum.COLLECT
        TmerAutoStart.Enabled = False
    End Sub

    Dim lastCollect As Date = Date.UtcNow

    Private ledBlink As Boolean = False

    Dim assetsToTrack As New List(Of AssetInfos) From {
        assetInfo("3USL")
    }

    Private frmMainResized As Boolean = False

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

        degiroLabel.Text = "cash: " & Math.Round(Degiro.accountCashMoula * 100) / 100 & "€ positions: " & Math.Round(100 * Degiro.accountPositionsMoula) / 100 & "€"



        If status = StatusEnum.COLLECT Then
            Dim diff As Integer = Math.Round(Date.UtcNow.Subtract(lastCollect).TotalSeconds)

            Label1.Text = "next price update " & (5 - diff) & " secs"

            If diff >= 5 Then
                TmrUI.Enabled = False
                Label1.Text = "updating ..."
                dbg.info("updating prices from trading view ...")
                fetchPrice(assetsToTrack)
                lastCollect = Date.UtcNow
                TmrUI.Enabled = True
            End If
        End If


        ' If status <> StatusEnum.SIMU Then GraphDraw.render()

        checkShutDown()

        If frmMainResized Then
            bottomGraph.render()

            frmMainResized = False
        End If

        Application.DoEvents()
    End Sub

    Public Sub checkShutDown()
        If Not CST.COMPILED Then Exit Sub
        Dim now As Date = Date.UtcNow
        If now.DayOfWeek <> DayOfWeek.Saturday And now.DayOfWeek <> DayOfWeek.Sunday Then Exit Sub

        'market close ?
        If now.Hour < 19 Then Exit Sub

        ' SLACK

        'shutdown !
        status = StatusEnum.OFFLINE
        ToolStripStatusSays.Text = "Going shutdown"
        Process.Start("ShutDown", "/s")
        Pause(1000)
        End
    End Sub


    ' ------------------------------------------------------------------------------------------------------
    ' Utils
    <DllImport("user32.dll")>
    Public Shared Function GetAsyncKeyState(ByVal vKey As System.Windows.Forms.Keys) As Short
    End Function


    Private Sub TestMeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TestMeToolStripMenuItem.Click
        fetchPrice(assetsToTrack)
    End Sub

    Private Sub StartCOLLECTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartCOLLECTToolStripMenuItem.Click
        status = StatusEnum.COLLECT
    End Sub

    Private Sub GraphRenderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GraphRenderToolStripMenuItem.Click
        bottomGraph.render()
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

    Private Sub RunToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RunToolStripMenuItem.Click
        simulateStupidAlgo()
    End Sub

    Private Sub FrmMain_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        frmMainResized = True
    End Sub
End Class
