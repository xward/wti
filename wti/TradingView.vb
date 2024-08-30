Imports System.Security.Policy

Module TradingView
    Public currentPrice As New Hashtable()

    ' need time when it was updated

    Public Sub fetchPrice(asset As AssetEnum)
        Dim infos As AssetInfos = assetInfo(asset)

        Edge.createTabIfNotExist(infos.ticker & " ", infos.tradingViewUrl, Edge.OpenModeEnum.AS_TAB)

        Dim s As String = KMOut.selectAllCopy()
        Dim val As Double = Nothing

        Try
            val = Double.Parse(s.Split(vbCrLf).ElementAt(0))
        Catch ex As Exception
            dbg.fail("Can't parse tradiview price of " & asset.ToString & " " & s)
        End Try

        currentPrice.Item(asset.ToString) = New AssetPrice With {.ticker = infos.ticker, .price = val, .dat = DateTime.UtcNow()}
        dbg.info(asset.ToString & " curent value " & val & " EUR")
    End Sub


    '    28.910
    'EUR
    '−1.950(−6.32%)
    'The objective Of this product Is To provide a total Return comprised Of three times the daily performance Of the NASDAQ Commodity 2nd Front Crude Oil ER index (the Benchmark), plus the interest revenue earned On the collateralised amount.
End Module
