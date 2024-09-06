Imports System.Security.Policy
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox
Imports System.IO


Module TradingView
    'fetch current prices from trading view
    ' futur (?): do it from a watchlist instead of one asset per edge tab

    Private priceList As New List(Of AssetPrice)

    Public Sub setPrice(asset As AssetInfos, price As AssetPrice)
        Dim previous As AssetPrice = getPrice(asset)

        ' a bit dirty
        If Not IsNothing(previous) Then priceList.Remove(previous)
        priceList.Add(price)

        Dim current As AssetPrice = getPrice(asset)

        dbg.info(current.ticker & " curent value " & current.price & ". Today change = " & current.todayChangePerc & "%")

        If previous.price <> current.price Then pushPriceToFile(asset)
    End Sub

    Public Function getPrice(asset As AssetInfos) As AssetPrice
        For Each p As AssetPrice In priceList
            If p.ticker = asset.ticker Then Return p
        Next

        Return Nothing
    End Function

    Public Sub fetchPrice(assetTickers As String())
        Dim assets As New List(Of AssetInfos)
        For Each ticker As String In assetTickers
            assets.Add(assetInfo(ticker))
        Next
        fetchPrice(assets)
    End Sub

    Public Sub fetchPrice(assets As List(Of AssetInfos))
        Dim start As Date = Date.UtcNow

        Dim skipAsset As New AssetInfos With {.ticker = "nothing"}

        'maybe the current tab opened is one of them
        For Each asset As AssetInfos In assets
            If Edge.currentEdgeWindowTitleInclude(asset.ticker) Then
                Edge.updateEdgeProcess()
                setFromTitle(asset)
                skipAsset = asset
            End If
        Next

        'do the rest
        For Each asset As AssetInfos In assets
            If asset.ticker = skipAsset.ticker Then Continue For
            Edge.createTabIfNotExist(asset.ticker & " ", asset.tradingViewUrl, Edge.OpenModeEnum.AS_TAB)
            setFromTitle(asset)
        Next

        dbg.info("Updated price of " & assets.Count & " assets within " & Math.Round(Date.UtcNow.Subtract(start).TotalMilliseconds) & "ms")
    End Sub


    Public Sub fetchPrice(assetTicker As String)
        fetchPrice(assetInfo(assetTicker))
    End Sub

    Public Sub fetchPrice(asset As AssetInfos)
        Dim l As New List(Of AssetInfos)
        l.Add(asset)
        fetchPrice(l)
    End Sub

    Public Sub pushPriceToFile(infos As AssetInfos)

        ' 280403 162431|23.56
        ' 04/28/2024 1:50:00 PM|28.35

        Dim fileName As String = CST.DATA_PATH & "/dataFromThePast/" & infos.ticker & "_" & Date.UtcNow.Year & "_" & Date.UtcNow.Month.ToString("00") & ".tv.txt"

        Dim price As AssetPrice = getPrice(infos)

        Dim line As String = price.dat.Day.ToString("00") & price.dat.Month.ToString("00") & price.dat.Year.ToString.Substring(2) & " " &
            price.dat.Hour.ToString("00") & price.dat.Minute.ToString("00") & price.dat.Second.ToString("00") & "|" & price.price & vbCrLf

        FrmMain.ListBoxEvents.Items.Add(infos.ticker & " " & line)
        FrmMain.ListBoxEvents.SelectedIndex = FrmMain.ListBoxEvents.Items.Count - 1

        If FrmMain.ListBoxEvents.Items.Count > 30 Then
            FrmMain.ListBoxEvents.Items.RemoveAt(0)
        End If

        If File.Exists(fileName) Then
            File.AppendAllText(fileName, line)
        Else
            'one file per month
            File.WriteAllText(fileName, line)
        End If

    End Sub


    'Public Sub updateAssetValueDataGridFor(asset As AssetEnum)
    '    'Dim infos As AssetInfos = assetInfo(asset)
    '    'Dim price As AssetPrice = currentPrice.Item(asset.ToString)

    '    '' update DataGridViewAssetPrices
    '    'For Each row As DataGridViewRow In FrmMain.DataGridViewAssetPrices.Rows
    '    '    If row.Cells(0).Value = infos.ticker Then
    '    '        row.Cells(1).Value = price.price
    '    '        row.Cells(2).Value = price.todayChangePerc
    '    '        Exit Sub
    '    '    End If
    '    'Next

    '    'FrmMain.DataGridViewAssetPrices.Rows.Add(New String() {infos.ticker, price.price, price.todayChangePerc})
    'End Sub

    Private Sub setFromTitle(asset As AssetInfos)

        Dim title As String = Edge.edgeProcess.MainWindowTitle

        Dim val As Double = Nothing
        Dim todayChangePerc As Double = Nothing

        If Not title.StartsWith(asset.ticker) Then
            dbg.fail("This is not a " & asset.ticker & " title: " & title)
        Else
            Try
                val = parseMoney(title.Split(" ").ElementAt(1))
                todayChangePerc = Double.Parse(title.Split(" ").ElementAt(3).Replace("%", "").Replace("−", "-"))
            Catch ex As Exception
                dbg.fail("Can't parse tradiview price of " & title)
            End Try

            setPrice(asset, New AssetPrice With {.ticker = asset.ticker, .price = val, .todayChangePerc = todayChangePerc, .dat = DateTime.UtcNow()})

            'currentPrice.Item(infos.ticker) = New AssetPrice With {.ticker = infos.ticker, .price = val, .todayChangePerc = todayChangePerc, .dat = DateTime.UtcNow()}

        End If
        ' 3OIL 28.910 ▼ −6.32% MSCI SP500 NASDAQ And 2 more pages - Personal - Microsoft​ Edge
        ' 3OIS 49.220 ▲ +6.62% Unnamed And 2 more pages - Personal - Microsoft​ Edge
    End Sub

    Private Sub setFromCtrlA(infos As AssetInfos)
        Dim s As String = KMOut.selectAllCopy()
        Dim val As Double = Nothing

        Try
            val = parseMoney(s.Split(vbCrLf).ElementAt(0))
        Catch ex As Exception
            dbg.fail("Can't parse tradiview price of " & infos.ticker & " " & s)
        End Try

        setPrice(infos, New AssetPrice With {.ticker = infos.ticker, .price = val, .todayChangePerc = Nothing, .dat = DateTime.UtcNow()})

        dbg.info(infos.ticker & " curent value " & val)
        '    28.910
        'EUR
        '−1.950(−6.32%)
        'The objective Of this product Is To provide a total Return comprised Of three times the daily performance Of the NASDAQ Commodity 2nd Front Crude Oil ER index (the Benchmark), plus the interest revenue earned On the collateralised amount.
    End Sub
End Module
