Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Tab
Imports WorstTradingInitiative.CST.CST

Public Class FrmMain
    ' je veux simuler du long terme sp500, tester des algos, visualiser feedback (vertical position in, position out, active orders), // graph dist from max ever
    '   [done] graph v1
    '   [done] load data from yahoo csv daily
    ' replayInit from date to date
    '   simuler, agir
    ' je veux voir wti, sp500 "live" data value, indice de peur



    ' screenshot whole screen 1/2 resolution, wti only, push to web page (fly.io?) auto-refresh-mdr-liveview
    ' 960x540 png

    ' --------------------------------------------------------------------------------------------------------
    'C:\Users\xwar\AppData\Local\Programs\Python\Python312\Scripts

    ' live visual graphs 3mo+5d
    ' je veux visualiser des data sur des annees, algo chute depuis maxever
    ' show price sur ui, max ever diff
    ' si sp500 < 17.5% max ever -> alert slack/sms
    ' analyze pattern tools (chute, stable ...)
    ' implem 4%/1.5% algo with, simulate stuffs

    ' degiro: place sell order, update order, delete order
    ' degiro: manage order too far from objective

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
        If Not CST.COMPILED And status = StatusEnum.OFFLINE And False Then Degiro.createFakeData()

        ' Edge init
        Edge.ensureRunning()
        ' Edge.printAllEdge()

        initUI()



        ' dbg.info(getPrice(AssetNameEnum.SP500_3X).ToString)

        'my current playground
        If CST.HOST_NAME = hostNameEnum.GALACTICA And False Then
            SP500StrategyLab.runAll()
        End If


        TmerAutoStart.Enabled = True
    End Sub


    Public bottomGraph As Graph


    Public Sub initUI()
        If CST.COMPILED Then runType.Text = "RUN" Else runType.Text = "DEBUG"

        Dim fullScreenMode As Boolean = CST.COMPILED

        Me.Width = Math.Min(Me.Width, CST.SCREEN.Width - Edge.edgeWindowRect.Width)

        ' Me
        Me.Top = 0
        If fullScreenMode Then Me.Left = Edge.edgeWindowRect.Width - 15 Else Me.Left = CST.SCREEN.Width - Me.Width
        If fullScreenMode Then Me.Width = CST.SCREEN.Width - Me.Left
        Me.Height = CST.SCREEN.Height

        Me.Text = "not WTI - " & CST.HOST_NAME.ToString


        PanelGraphTop.Height = 0 'TopPanel.Height / 2

        ' auto start configuration
        If CST.COMPILED And CommandLineArgs.Count > 0 AndAlso CommandLineArgs(0) = "COLLECT" Then
            ' nothing can be auto start for now

            ' statusLed.BackgroundImage = PictureLedGreenOn.Image
            ' ToolStripStatusSays.Text = "About to auto-start with action COLLECT"
            ' TmerAutoStart.Enabled = True
        End If

        Select Case CST.HOST_NAME
            Case hostNameEnum.GALACTICA
                bottomGraph = New Graph(PanelGraphBottom, AssetNameEnum.SP500)
            Case hostNameEnum.GHOST
                bottomGraph = New Graph(PanelGraphBottom, AssetNameEnum.SP500_3X)
        End Select

        Degiro.updateTradePanelUI()

    End Sub

    Private Sub TmerKeyIput_Tick(sender As Object, e As EventArgs) Handles TmerKeyIput.Tick
        If GetAsyncKeyState(Keys.F2) And My.Computer.Keyboard.CtrlKeyDown Then
            ' nothing to start, yet
        End If
        If GetAsyncKeyState(Keys.F3) And My.Computer.Keyboard.CtrlKeyDown Then
            status = StatusEnum.INTERRUPT
            marketPriceStop()
            'prevent auto start at boot
            TmerAutoStart.Enabled = False
            ToolStripStatusSays.Text = "Interrupt by user"
        End If
    End Sub

    Private Sub TmerAutoStart_Tick(sender As Object, e As EventArgs) Handles TmerAutoStart.Tick
        TmerAutoStart.Enabled = False
        Dim start As Date = Date.UtcNow
        ToolStripStatusSays.Text = "init marketprice ..."
        Application.DoEvents()
        marketPriceStart()
        ToolStripStatusSays.Text = "loaded within " & Math.Round(Date.UtcNow.Subtract(start).TotalMilliseconds) & " ms"
    End Sub


    Private ledBlink As Boolean = False
    Private esterAlertBlink As Boolean = False

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
            Case StatusEnum.SIMU, StatusEnum.LIVE
                If ledBlink Then
                    statusLed.BackgroundImage = PictureLedGreenOn.Image
                Else
                    statusLed.BackgroundImage = PictureLedGreenOff.Image
                End If
                ledBlink = Not ledBlink
        End Select

        If Ester.rate < 3 Then
            esterAlertBlink = Not esterAlertBlink
            If esterAlertBlink Then esterLabel.BackColor = Color.IndianRed Else esterLabel.BackColor = Color.Transparent
        End If

        degiroLabel.Text = "cash: " & Math.Round(Degiro.accountCashMoula * 100) / 100 & "€ positions: " & Math.Round(100 * Degiro.accountPositionsMoula) / 100 & "€"



        ' If status <> StatusEnum.SIMU Then GraphDraw.render()

        checkShutDown()

        If frmMainResized Then

            dbg.info("resizing")
            If Not IsNothing(bottomGraph) Then bottomGraph.render()

            frmMainResized = False
        End If

        Application.DoEvents()
    End Sub

    Public Sub checkShutDown()
        If Not CST.COMPILED Then Exit Sub
        Dim now As Date = Date.UtcNow

        'market close ?
        If now.Hour < 17 And now.DayOfWeek <> DayOfWeek.Saturday And now.DayOfWeek <> DayOfWeek.Sunday Then Exit Sub

        ' SLACK

        'shutdown !
        status = StatusEnum.OFFLINE
        ToolStripStatusSays.Text = "Going shutdown"
        Process.Start("ShutDown", "/s")
        Pause(1000)
        End
    End Sub

    Public Sub pushLineToListBox(str As String)
        ListBoxLogEvents.Items.Add(str)
        ListBoxLogEvents.SelectedIndex = ListBoxLogEvents.Items.Count - 1

        If ListBoxLogEvents.Items.Count > 30 Then ListBoxLogEvents.Items.RemoveAt(0)

    End Sub


    ' ------------------------------------------------------------------------------------------------------
    ' Utils
    <DllImport("user32.dll")>
    Public Shared Function GetAsyncKeyState(ByVal vKey As System.Windows.Forms.Keys) As Short
    End Function


    Private Sub TestMeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TestMeToolStripMenuItem.Click
        TradingView.fetchPrice(assetsToTrack)
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
        SP500.runPocSpx()
    End Sub

    Private Sub FrmMain_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        frmMainResized = True
    End Sub


    Private Sub FetchSpxFromYahooToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FetchSpxFromYahooToolStripMenuItem.Click
        Yahoo.fetchPrice(assetFromName(AssetNameEnum.SP500))
    End Sub
End Class
