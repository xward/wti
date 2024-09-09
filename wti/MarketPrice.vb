Imports System.IO
Module MarketPrice

    Private assetsPriceHistory As New List(Of AssetHistory)

    Public Sub setCurrentPrice(asset As AssetInfos, current As AssetPrice)
        Dim previous As AssetPrice = getPrice(asset)

        If IsNothing(previous) OrElse previous.price <> current.price Then
            Dim assHistory As AssetHistory = getAssetHistory(asset)
            assHistory.addPrice(current)

            dbg.info(current.ticker & " curent value " & current.price & ". Today change = " & current.todayChangePerc & "%")
            If asset.persistHistory Then pushPriceToFile(asset)
        Else
            dbg.info(current.ticker & "price no change")
        End If
    End Sub

    Public Function getPrice(asset As AssetInfos) As AssetPrice
        Return getAssetHistory(asset).currentPrice
    End Function

    'Public Function allPricesAfter(asset As AssetInfos, dat As Date) As List(Of AssetPrice)
    '    Return getAssetHistory(asset).allPricesAfter(dat)
    'End Function

    Public Function getAssetHistory(asset As AssetInfos) As AssetHistory

        For Each a As AssetHistory In assetsPriceHistory
            If a.asset.ticker = asset.ticker Then Return a
        Next

        Dim ass As AssetHistory = New AssetHistory(asset)
        assetsPriceHistory.Add(ass)
        Return ass
    End Function

    ' -------------------------------------------------------------------------------------------------------------------------------------
    ' Replay

    Public Sub replayInit(asset As AssetInfos)
        getAssetHistory(asset).initReplay()
    End Sub

    Public Function replayNext(asset As AssetInfos) As Boolean
        Return getAssetHistory(asset).replayNext()
    End Function

    ' -------------------------------------------------------------------------------------------------------------------------------------

    Private Sub pushPriceToFile(infos As AssetInfos)

        ' 280403 162431|23.56
        ' 04/28/2024 1:50:00 PM|28.35

        Dim price As AssetPrice = getPrice(infos)
        Dim line As String = price.Serialize()

        FrmMain.pushLineToListBox(infos.ticker & " " & line)

        If status = StatusEnum.SIMU Then Exit Sub

        Dim fileName As String = CST.DATA_PATH & "/dataFromThePast/" & infos.ticker & "_" & Date.UtcNow.Year & "_" & Date.UtcNow.Month.ToString("00") & ".tv.txt"

        If CST.HOST_NAME = CST.CST.hostNameEnum.GALACTICA Then
            FrmMain.pushLineToListBox("skip save to file because we are galactica")
            Exit Sub
        End If

        If File.Exists(fileName) Then
            File.AppendAllText(fileName, line & vbCrLf)
        Else
            'one file per month
            File.WriteAllText(fileName, line & vbCrLf)
        End If
    End Sub
End Module


