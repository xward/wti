﻿Imports System.IO
Imports System.Reflection.Emit
Imports WorstTradingInitiative.Structures
Module MarketPrice

    Private assetsPriceHistory As New List(Of AssetHistory)

    Public Sub setCurrentPrice(asset As AssetInfos, current As AssetPrice)
        getAssetHistory(asset).addPrice(current)


        'Dim previous As AssetPrice = getPrice(asset)

        'If IsNothing(previous) OrElse previous.price <> current.price Then
        '    Dim assHistory As AssetHistory = getAssetHistory(asset)
        '    assHistory.addPrice(current)

        '    dbg.info(current.ticker & " curent value " & current.price & ". Today change = " & current.todayChangePerc & "%")
        '    If asset.persistHistory Then pushPriceToFile(asset)
        'Else
        '    dbg.info(current.ticker & "price no change")
        'End If
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
        addAssetHistory(AssetNameEnum.SP500)
        addAssetHistory(AssetNameEnum.PEA_SP500)
        startTimer()
    End Sub


    Private Function addAssetHistory(assetName As AssetNameEnum) As AssetHistory
        Return addAssetHistory(assetFromName(assetName))
    End Function
    Private Function addAssetHistory(asset As AssetInfos) As AssetHistory
        For Each a In assetsPriceHistory
            If a.asset.ticker = asset.ticker Then Return a
        Next
        Dim ass As AssetHistory = New AssetHistory(asset)
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

        'If status = StatusEnum.COLLECT Then
        '    Dim diff As Integer = Math.Round(Date.UtcNow.Subtract(lastCollect).TotalSeconds)

        '    FrmMain.Label1.Text = "next price update " & (5 - diff) & " secs"

        '    If diff >= 5 Then
        '        FrmMain.Label1.Text = "updating ..."
        '        dbg.info("updating prices from trading view ...")
        '        TradingView.fetchPrice(assetsToTrack)
        '        lastCollect = Date.UtcNow
        '        'TmrUI.Enabled = True

        '        FrmMain.bottomGraph.render()
        '    End If
        'End If

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


