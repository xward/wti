Imports System.IO

Public Class AssetHistory
    Public asset As AssetInfos

    Private prices As New List(Of AssetPrice)

    ' max price among prices stored in Me
    Private maxPrice As AssetPrice
    Public diffWithMaxPerc As Double
    Private maxPriceEver As AssetPrice

    Private lastDataSourceUpdate As Date

    Private doingReplay As Boolean = False
    Private replayIndex As Integer


    Public Sub New(asset As AssetInfos)
        Me.asset = asset
        loadMaxEver()
        If asset.persistHistory Then loadDataFromPersistHistory()



        ' load sp500 daily data, using special fonction provided from SP500 module

        lastDataSourceUpdate = Date.UtcNow
    End Sub

    Public Overrides Function ToString() As String
        Return StructToString(Me)
    End Function

    ' ---------------------------------------------------------------------------------------------------------------------------
    ' API

    Public Function currentPrice() As AssetPrice
        If doingReplay Then
            Return prices.ElementAt(replayIndex)
        ElseIf prices.Count > 0 Then
            Return prices.Last

        Else
            Return Nothing
        End If
    End Function

    Public Sub addPrice(ByRef price As AssetPrice)
        Dim current As AssetPrice = currentPrice()
        If Not IsNothing(current) AndAlso current.price = price.price Then Exit Sub

        FrmMain.pushLineToListBox(asset.ticker & " " & price.Serialize)

        dbg.info(price.ticker & " curent value " & price.price & ". Today change = " & price.todayChangePerc & "%")
        If asset.persistHistory Then
            pushPriceToFile()
        Else
            FrmMain.pushLineToListBox("skip save " & price.ticker & " persistHistory is off")
        End If


        If IsNothing(maxPrice) OrElse price.price > maxPrice.price Then maxPrice = price
        If IsNothing(maxPriceEver) OrElse price.price > maxPriceEver.price Then
            maxPriceEver = price
            pushMaxEverFile()
        End If
        prices.Add(price)
    End Sub

    Public Function allPricesAfter(dat As Date) As List(Of AssetPrice)
        Dim np As New List(Of AssetPrice)
        If doingReplay Then
            For Each p As AssetPrice In prices.Slice(0, replayIndex + 1)
                If p.dat.CompareTo(dat) > 0 Then np.Add(p)
            Next
        Else
            For Each p As AssetPrice In prices
                If p.dat.CompareTo(dat) > 0 Then np.Add(p)
            Next
        End If
        Return np
    End Function


    ' ---------------------------------------------------------------------------------------------------------------------------
    ' DATA LOADER

    Private Sub loadMaxEver()
        Dim fileName As String = CST.DATA_PATH & "/dataFromThePast/" & asset.ticker & ".max.ever.txt"
        If File.Exists(fileName) Then
            Dim c As String = File.ReadAllText(fileName)
            maxPriceEver = AssetPrice.Deserialize(asset, c)
        Else
            maxPriceEver = New AssetPrice
            maxPriceEver.price = 0
        End If
    End Sub

    Private Sub loadDataFromPersistHistory()
        prices.Clear()

        For Each filePath As String In Directory.GetFiles(CST.DATA_PATH & "/dataFromThePast/")
            If Not filePath.Contains(asset.ticker) Then Continue For
            If Not filePath.Contains(".tv.txt") Then Continue For
            For Each line In File.ReadAllLines(filePath)
                addPrice(AssetPrice.Deserialize(asset, line))
            Next
        Next

        dbg.info("AssetHistory " & asset.ticker & " loaded " & prices.Count & " historical prices")

        prices.Sort(New AssetPriceDateComparer)
    End Sub

    Private Sub loadFromCustomFile()
        'daily, like sp500
    End Sub

    ' ---------------------------------------------------------------------------------------------------------------------------
    ' LIVE DATA FETCH FROM SOURCE

    Public lastDataFetchFromSource As Date = Date.UtcNow.AddDays(-1)

    Public Sub fetchDataFromSource()
        If asset.updateDateFromSource = False Then Exit Sub

        FrmMain.Label1.Text &= asset.ticker & " (" & Math.Max(0, Math.Ceiling(asset.updatePeriodSec - Date.UtcNow.Subtract(lastDataFetchFromSource).TotalSeconds)) & "secs)"
        If Date.UtcNow.Subtract(lastDataFetchFromSource).TotalSeconds < asset.updatePeriodSec Then Exit Sub

        ' dbg.info("fetching data from source for " & asset.name.ToString)

        Select Case asset.updateSource
            Case UpdateSourceEnum.TRADING_VIEW
                ' opti: we could update all trading view assets at once
                TradingView.fetchPrice(asset)
            'Case UpdateSourceEnum.YAHOO

                'Yahoo.fetchPrice(asset)
            Case UpdateSourceEnum.BOURSOBANK

        End Select

        ' hack for now
        If asset.ticker = FrmMain.bottomGraph.asset.ticker Then
            FrmMain.bottomGraph.render()
        End If

        lastDataFetchFromSource = Date.UtcNow
    End Sub


    ' ---------------------------------------------------------------------------------------------------------------------------
    ' DATA SAVE

    Private Sub pushMaxEverFile()
        FrmMain.pushLineToListBox(asset.ticker & " new max value ever " & maxPriceEver.price)

        Dim fileName As String = CST.DATA_PATH & "/dataFromThePast/" & asset.ticker & ".max.ever.txt"
        File.WriteAllText(fileName, maxPriceEver.Serialize())
    End Sub

    Private Sub pushPriceToFile()

        ' 280403 162431|23.56
        ' 04/28/2024 1:50:00 PM|28.35

        Dim price As AssetPrice = currentPrice()
        Dim line As String = price.Serialize()

        If status = StatusEnum.SIMU Then Exit Sub

        Dim fileName As String = CST.DATA_PATH & "/dataFromThePast/" & asset.ticker & "_" & Date.UtcNow.Year & "_" & Date.UtcNow.Month.ToString("00") & ".tv.txt"

        If File.Exists(fileName) Then
            File.AppendAllText(fileName, line & vbCrLf)
        Else
            'one file per month
            File.WriteAllText(fileName, line & vbCrLf)
        End If
    End Sub

    ' ---------------------------------------------------------------------------------------------------------------------------
    ' REPLAY

    ' todo: optional startDate
    Public Sub initReplay()
        doingReplay = True
        replayIndex = 0
        maxPrice = prices.First
    End Sub

    Public Function replayNext() As Boolean
        ' thus we will never use the last value
        If replayIndex = prices.Count - 1 Then Return False
        replayIndex += 1
        If currentPrice().price > maxPrice.price Then maxPrice = currentPrice()
        Return True
    End Function

    'Public Function minPriceEver() As AssetPrice
    '    If doingReplay Then
    '        maxPrice = prices.ElementAt(0)
    '        For Each p As AssetPrice In prices.Slice(1, replayIndex + 1)
    '            If maxPrice.price < p.price Then maxPrice = p
    '        Next
    '    End If
    '    Return maxPrice
    'End Function

    'Public Function maxPriceEver() As AssetPrice
    '    If doingReplay Then
    '        maxPrice = prices.ElementAt(0)
    '        For Each p As AssetPrice In prices.Slice(1, replayIndex + 1)
    '            If maxPrice.price < p.price Then maxPrice = p
    '        Next
    '    End If
    '    Return maxPrice
    'End Function

End Class



Public Class AssetPriceDateComparer
    Implements IComparer(Of AssetPrice)

    Public Function Compare(x As AssetPrice, y As AssetPrice) As Integer Implements IComparer(Of AssetPrice).Compare
        Return x.dat.CompareTo(y.dat)
    End Function
End Class