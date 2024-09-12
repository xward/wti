Imports System.IO
Imports System.Reflection.Emit
Imports WorstTradingInitiative.Structures

Module MarketPrice

    Public assetsPriceHistory As New List(Of AssetHistory)

    ' from live data fetchers only
    Public Sub setCurrentPrice(asset As AssetInfos, newPrice As AssetPrice)
        Dim assetHistory as AssetHistory = getAssetHistory(asset)
        Dim current As AssetPrice = assetHistory.currentPrice()

        ' exit if price didn't change
        If Not IsNothing(current) AndAlso current.price = newPrice.price Then Exit Sub

        ' new price log + persist
        FrmMain.pushLineToListBox(asset.ticker & " " & newPrice.Serialize)
        ' dbg.info(price.ticker & " curent value " & newPrice.price & ". Today change = " & newPrice.todayChangePerc & "%")
        If asset.persistHistoryFromLiveCollect Then
            assetHistory.pushPriceToFile()
        Else
            FrmMain.pushLineToListBox("skip save " & newPrice.ticker & " persistHistoryFromLiveCollect is off")
        End If

        assetHistory.addPrice(newPrice)
    End Sub

    Public Function getPrice(asset As AssetInfos) As AssetPrice
        Return getAssetHistory(asset).currentPrice
    End Function

    Public Function getPrice(assetName As AssetNameEnum) As AssetPrice
        Return getPrice(assetInfo(assetName))
    End Function

    Public Function getPrice(tickerName As String) As AssetPrice
        Return getPrice(assetInfo(tickerName))
    End Function

    'Public Function allPricesAfter(asset As AssetInfos, dat As Date) As List(Of AssetPrice)
    '    Return getAssetHistory(asset).allPricesAfter(dat)
    'End Function

    Public Function getAssetHistory(asset As AssetInfos) As AssetHistory
        For Each a As AssetHistory In assetsPriceHistory
            If a.asset.ticker = asset.ticker Then Return a
        Next

        Return addAssetHistory(asset)
    End Function

    ' -------------------------------------------------------------------------------------------------------------------------------------
    ' asset histories updates
    Private marketPriceTmer As Timer

    Dim lastCollect As Date = Date.UtcNow

    Dim assetsToTrack As New List(Of AssetInfos) From {
        assetInfo(AssetNameEnum.SP500_3X)
    }

    Public Sub marketPriceStart()

        ' update ester rate
        Ester.fetchRateFromBCE()
        FrmMain.esterLabel.Text = "ester: " & Ester.rate

        addAssetHistory(AssetNameEnum.SP500_3X)

        ' pas necessaire pour le moment, ca plombe le chargement
        ' addAssetHistory(AssetNameEnum.SP500)
        ' addAssetHistory(AssetNameEnum.PEA_SP500)
        startTimer()
    End Sub

    Private Function addAssetHistory(assetName As AssetNameEnum) As AssetHistory
        Return addAssetHistory(assetFromName(assetName))
    End Function

    Private Function addAssetHistory(asset As AssetInfos) As AssetHistory
        For Each a In assetsPriceHistory
            If a.asset.ticker = asset.ticker Then Return a
        Next
        ' this will load data from file, if any
        Dim ass As AssetHistory = New AssetHistory(asset)
        Application.DoEvents()
        assetsPriceHistory.Add(ass)
        Return ass
    End Function

    Private Sub startTimer()
        marketPriceTmer = New Timer
        marketPriceTmer.Interval = 100
        AddHandler marketPriceTmer.Tick, AddressOf MarketPriceTmer_Tick
        marketPriceTmer.Enabled = True
    End Sub

    Private Sub MarketPriceTmer_Tick(sender As Object, e As EventArgs)
        FrmMain.Label1.Text = ""

        ' for each assetsPriceHistory, fetch data in enabled
        For Each ass As AssetHistory In assetsPriceHistory
            ass.fetchDataFromSource()
        Next
    End Sub


    ' -------------------------------------------------------------------------------------------------------------------------------------
    ' Replay

    Public Sub replayInit(asset As AssetInfos)
        getAssetHistory(asset).initReplay()
    End Sub

    Public Function replayNext(asset As AssetInfos) As Boolean
        Return getAssetHistory(asset).replayNext()
    End Function

    ' -------------------------------------------------------------------------------------------------------------------------------------


End Module
