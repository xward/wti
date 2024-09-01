Imports System.Security.Policy
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox

Module TradingView
    'fetch current prices from trading view
    ' futur (?): do it from a watchlist instead of one asset per edge tab

    Public currentPrice As New Hashtable()

    Public Sub fetchPrice(asset As AssetEnum)
        Dim start As Date = Date.UtcNow
        Dim infos As AssetInfos = assetInfo(asset)

        Edge.createTabIfNotExist(infos.ticker & " ", infos.tradingViewUrl, Edge.OpenModeEnum.AS_TAB)

        ' setFromCtrlA(asset)
        setFromTitle(asset)

        updateAssetValueDataGridFor(asset)

        dbg.info("Updated asset price of " & asset.ToString & " data within " & Math.Round(Date.UtcNow.Subtract(start).TotalMilliseconds) & "ms")
    End Sub

    Public Sub updateAssetValueDataGridFor(asset As AssetEnum)
        'Dim infos As AssetInfos = assetInfo(asset)
        'Dim price As AssetPrice = currentPrice.Item(asset.ToString)

        '' update DataGridViewAssetPrices
        'For Each row As DataGridViewRow In FrmMain.DataGridViewAssetPrices.Rows
        '    If row.Cells(0).Value = infos.ticker Then
        '        row.Cells(1).Value = price.price
        '        row.Cells(2).Value = price.todayChangePerc
        '        Exit Sub
        '    End If
        'Next

        'FrmMain.DataGridViewAssetPrices.Rows.Add(New String() {infos.ticker, price.price, price.todayChangePerc})
    End Sub

    Private Sub setFromTitle(asset As AssetEnum)
        Dim infos As AssetInfos = assetInfo(asset)
        Dim title As String = Edge.edgeProcess.MainWindowTitle

        Dim val As Double = Nothing
        Dim todayChangePerc As Double = Nothing

        If Not title.StartsWith(infos.ticker) Then
            dbg.fail("This is not a " & infos.ticker & " title: " & title)
        Else
            Try
                val = parseMoney(title.Split(" ").ElementAt(1))
                todayChangePerc = Double.Parse(title.Split(" ").ElementAt(3).Replace("%", "").Replace("−", "-"))
            Catch ex As Exception
                dbg.fail("Can't parse tradiview price of " & title)
            End Try

            currentPrice.Item(asset.ToString) = New AssetPrice With {.ticker = infos.ticker, .price = val, .todayChangePerc = todayChangePerc, .dat = DateTime.UtcNow()}
            dbg.info(asset.ToString & " curent value " & val & " EUR. Today change = " & todayChangePerc & "%")
        End If
        ' 3OIL 28.910 ▼ −6.32% MSCI SP500 NASDAQ And 2 more pages - Personal - Microsoft​ Edge
        ' 3OIS 49.220 ▲ +6.62% Unnamed And 2 more pages - Personal - Microsoft​ Edge
    End Sub

    Private Sub setFromCtrlA(asset As AssetEnum)
        Dim infos As AssetInfos = assetInfo(asset)
        Dim s As String = KMOut.selectAllCopy()
        Dim val As Double = Nothing

        Try
            val = parseMoney(s.Split(vbCrLf).ElementAt(0))
        Catch ex As Exception
            dbg.fail("Can't parse tradiview price of " & asset.ToString & " " & s)
        End Try

        currentPrice.Item(asset.ToString) = New AssetPrice With {.ticker = infos.ticker, .price = val, .todayChangePerc = Nothing, .dat = DateTime.UtcNow()}
        dbg.info(asset.ToString & " curent value " & val & " EUR")
        '    28.910
        'EUR
        '−1.950(−6.32%)
        'The objective Of this product Is To provide a total Return comprised Of three times the daily performance Of the NASDAQ Commodity 2nd Front Crude Oil ER index (the Benchmark), plus the interest revenue earned On the collateralised amount.
    End Sub
End Module
