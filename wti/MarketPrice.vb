Imports System.IO
Module MarketPrice

    Private assetsPriceHistory As New List(Of AssetHistory)

    Public Sub setPrice(asset As AssetInfos, price As AssetPrice)
        Dim previous As AssetPrice = getPrice(asset)

        Dim assHistory As AssetHistory = assetHistory(asset)
        assHistory.prices.Add(price)

        Dim current As AssetPrice = getPrice(asset)

        dbg.info(current.ticker & " curent value " & current.price & ". Today change = " & current.todayChangePerc & "%")

        If IsNothing(previous) OrElse previous.price <> current.price Then pushPriceToFile(asset)
    End Sub

    Public Function getPrice(asset As AssetInfos) As AssetPrice
        Return assetHistory(asset).currentPrice
    End Function

    Public Function assetHistory(asset As AssetInfos) As AssetHistory
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
        assetHistory(asset).initReplay()
    End Sub

    Public Function replayNext(asset As AssetInfos) As Boolean
        Return assetHistory(asset).replayNext()
    End Function

    ' -------------------------------------------------------------------------------------------------------------------------------------

    Private Sub pushPriceToFile(infos As AssetInfos)

        ' 280403 162431|23.56
        ' 04/28/2024 1:50:00 PM|28.35

        Dim price As AssetPrice = getPrice(infos)
        Dim line As String = price.Serialize()

        FrmMain.ListBoxLogEvents.Items.Add(infos.ticker & " " & line)
        FrmMain.ListBoxLogEvents.SelectedIndex = FrmMain.ListBoxLogEvents.Items.Count - 1

        If FrmMain.ListBoxLogEvents.Items.Count > 30 Then FrmMain.ListBoxLogEvents.Items.RemoveAt(0)

        If status = StatusEnum.SIMU Then Exit Sub

        Dim fileName As String = CST.DATA_PATH & "/dataFromThePast/" & infos.ticker & "_" & Date.UtcNow.Year & "_" & Date.UtcNow.Month.ToString("00") & ".tv.txt"

        If File.Exists(fileName) Then
            File.AppendAllText(fileName, line & vbCrLf)
        Else
            'one file per month
            File.WriteAllText(fileName, line & vbCrLf)
        End If
    End Sub
End Module


