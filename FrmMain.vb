Imports System.Runtime.InteropServices

Public Class FrmMain
    <DllImport("user32.dll")>
    Public Shared Function GetAsyncKeyState(ByVal vKey As System.Windows.Forms.Keys) As Short
    End Function

    ' // show asset prices
    ' orders, position, transaction, merged as trade
    ' show trades, with current price, how far I am

    ' simulation, place fake order, fetch fake order/position/transaction, output results to file
    ' implem 4%/1.5% algo with


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


    Dim lastFetch As Date = Date.UtcNow

    Private ledBlink As Boolean = False

    Dim assetsToTrack As New List(Of AssetInfos) From {
        assetInfo("3OIL"),
        assetInfo("3OIS")
    }

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles TmrUI.Tick
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
            Dim diff As Integer = Math.Round(Date.UtcNow.Subtract(lastFetch).TotalSeconds)

            Label1.Text = "next price update " & (5 - diff) & " secs"

            If diff >= 5 Then
                TmrUI.Enabled = False
                Label1.Text = "updating ..."
                dbg.info("updating prices from trading view ...")
                fetchPrice(assetsToTrack)
                lastFetch = Date.UtcNow
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


    Private Sub BtnTest_Click(sender As Object, e As EventArgs) Handles BtnTest.Click
        ' Degiro.updateAll()

        ' Edge.bringToFront()
        '  Edge.switchTab("3OIL")

        fetchPrice(assetsToTrack)
        'Degiro.setupWindows()
        'Edge.createTab("ddg.gg", Edge.OpenModeEnum.AS_WINDOW)
        'Edge.createTabIfNotExist("youtube", "https://www.youtube.com/", Edge.OpenModeEnum.AS_WINDOW, New Rectangle(3, 3, 500, 500))
    End Sub

    Private Sub Timer1_Tick_1(sender As Object, e As EventArgs) Handles TmerStartStop.Tick
        If GetAsyncKeyState(Keys.F2) And GetAsyncKeyState(Keys.ControlKey) Then
            status = StatusEnum.COLLECT
        End If
        If GetAsyncKeyState(Keys.F3) And GetAsyncKeyState(Keys.ControlKey) Then
            status = StatusEnum.OFFLINE
        End If
    End Sub
End Class
