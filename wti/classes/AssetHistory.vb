﻿Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip

Public Class AssetHistory
    Public asset As AssetInfos

    Private prices As New List(Of AssetPrice)

    ' max ever from file, even from outside timeframe
    Public maxPriceEver As AssetPrice
    ' max price ever before latest price
    Private maxPrice As New AssetPrice

    Private lastDataSourceUpdate As Date

    Private doingReplay As Boolean = False
    Private replayIndex As Integer


    Public Sub New(asset As AssetInfos)
        Me.asset = asset
        loadMaxEver()

        ' load prices
        prices.Clear()
        If asset.populateDataFromYahooDaily Then loadDataFromYahooDaily()
        If asset.populateDataFromYahooMinute Then loadDataFromYahooMinute()

        ' if any, load it
        loadDataFromPersistHistoryFromLiveCollect()

        ' sort
        prices.Sort(New AssetPriceDateComparer)

        ' load sp500 daily data, using special fonction provided from SP500 module

        ' purge/clean/merge data

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

    Public Function daySpanSize() As Double
        Return prices.Last.dat.Subtract(prices.First.dat).TotalDays
    End Function

    Public Function pricesCount() As Integer
        Return prices.Count
    End Function

    Public Function oldestPrice() As AssetPrice
        Return prices.First
    End Function

    ' to make suuure data engine is not down and we place an order for that
    Public Function lastUpdateAgoSec() As Double
        Return Date.UtcNow().Subtract(lastDataSourceUpdate).TotalSeconds
    End Function

    ' only MarketPrice or Me can call me
    Public Sub addPrice(ByRef price As AssetPrice)
        If IsNothing(maxPrice) OrElse price.price > maxPrice.price Then maxPrice = price
        If IsNothing(maxPriceEver) OrElse maxPrice.price > maxPriceEver.price Then
            maxPriceEver = maxPrice
            pushMaxEverFile()
        End If

        lastDataSourceUpdate = Date.UtcNow()

        price.currentMaxPrice = maxPrice.price
        price.currentMaxPriceDate = maxPrice.dat

        prices.Add(price)
    End Sub

    Public Function allPricesBetween(fromDate As Date, toDate As Date) As List(Of AssetPrice)
        Dim np As New List(Of AssetPrice)
        If doingReplay Then
            For Each p As AssetPrice In prices.Slice(0, replayIndex + 1)
                If p.dat.CompareTo(fromDate) > 0 And p.dat.CompareTo(toDate) < 0 Then np.Add(p)
            Next
        Else
            For Each p As AssetPrice In prices
                If p.dat.CompareTo(fromDate) > 0 And p.dat.CompareTo(toDate) < 0 Then np.Add(p)
                'If p.dat.Subtract(dat).TotalSeconds > 0 Then np.Add(p)
            Next
        End If
        Return np
    End Function


    ' ---------------------------------------------------------------------------------------------------------------------------
    ' DATA LOADER


    Private tmpMaxPrice As AssetPrice

    Private Sub loadMaxEver()
        Dim fileName As String = CST.DATA_PATH & "/dataFromThePast/" & asset.ticker & ".max.ever.txt"
        If File.Exists(fileName) Then
            Dim c As String = File.ReadAllText(fileName)
            maxPriceEver = AssetPrice.Deserialize(asset, c)
        Else
            maxPriceEver = New AssetPrice
            maxPriceEver.price = 0
        End If
        maxPrice = maxPriceEver
    End Sub


    Private Sub loadDataFromPersistHistoryFromLiveCollect()
        Dim count As Integer = 0

        For Each filePath As String In Directory.GetFiles(CST.DATA_PATH & "/dataFromThePast/")
            If Not filePath.Contains(asset.ticker) Then Continue For
            If Not filePath.Contains(".tv.txt") Then Continue For
            For Each line In File.ReadAllLines(filePath)
                Dim p As AssetPrice = AssetPrice.Deserialize(asset, line)
                If p.price > maxPrice.price Then maxPrice = p

                p.setCurrentMaxPrice(maxPrice)
                addPrice(p)
                count += 1
            Next
        Next

        dbg.info("AssetHistory " & asset.ticker & " loaded " & count & " historical prices from live collect")
    End Sub

    Private Sub loadDataFromYahooMinute()
        Dim count As Integer = 0
        tmpMaxPrice = New AssetPrice
        tmpMaxPrice.price = 0
        For Each filePath As String In Directory.GetFiles(CST.DATA_PATH & "/yahoo/")
            If Not filePath.Contains(asset.yahooTicker) Then Continue For
            If Not filePath.Contains(".minute.yahoo.txt") Then Continue For
            For Each line In File.ReadAllLines(filePath)
                count += loadFromAYahooFile(filePath)
            Next
        Next
        dbg.info("AssetHistory " & asset.ticker & " loaded " & count & " prices from yahoo minute history")
    End Sub

    Private Sub loadDataFromYahooDaily()
        Dim filePath As String = CST.DATA_PATH & "/yahoo/" & asset.yahooTicker & ".daily.yahoo.txt"
        Dim count As Integer = 0
        tmpMaxPrice = New AssetPrice
        tmpMaxPrice.price = 0
        If File.Exists(filePath) Then
            count += loadFromAYahooFile(filePath)

            dbg.info("AssetHistory " & asset.ticker & " loaded " & count & " prices from yahoo daily history")
        Else
            dbg.fail("[" & asset.ticker & "] No daily yahoo file found")
        End If
    End Sub


    Private Function loadFromAYahooFile(filePath As String) As Integer
        Dim daily As Boolean = False
        Dim count As Integer
        Dim openPrice As Double = -1

        Dim tmpPrice As AssetPrice


        For Each line In File.ReadAllLines(filePath)
            If line.Contains("Date;") Then
                daily = True
                Continue For
            End If
            If line.Contains("Datetime;") Then Continue For
            Dim split As String() = line.Split(";")

            ' daily
            ' Date;Open;High;Low;Close;Adj Close;Volume
            ' 1970-01-02;0.0;93.54000091552734;91.79000091552734;93.0;93.0;8050000
            ' minute
            ' Datetime;Open;High;Low;Close;Adj Close;Volume
            ' 2024-09-09 09:30:00-04:00;5442.06982421875;5450.18994140625;5442.06982421875;5448.89013671875;5448.89013671875;0


            Dim datChunk As String = split.ElementAt(0)

            Dim openDate As Date
            Dim closeDate As Date
            Dim step1Date As Date
            Dim step2Date As Date

            If daily Then
                openDate = Date.Parse(datChunk & " 09:30:00")
                closeDate = Date.Parse(datChunk & " 16:00:00")
                step1Date = openDate.AddHours(2)
                step2Date = openDate.AddHours(4)
            Else
                openDate = Date.Parse(datChunk)
                closeDate = openDate.AddSeconds(59)
                step1Date = openDate.AddSeconds(20)
                step2Date = openDate.AddSeconds(40)
            End If


            Dim open As Double = Double.Parse(split.ElementAt(1))
            Dim high As Double = Double.Parse(split.ElementAt(2))
            Dim low As Double = Double.Parse(split.ElementAt(3))
            Dim close As Double = Double.Parse(split.ElementAt(4))


            If openPrice = -1 Then openPrice = open

            challengeTmpMaxPrice(openPrice, openDate)

            If openPrice > 0 Then
                ' open price
                tmpPrice = New AssetPrice
                With tmpPrice
                    .ticker = asset.ticker
                    .price = open
                    .dat = openDate
                    .todayChangePerc = 0
                End With
                tmpPrice.setCurrentMaxPrice(tmpMaxPrice)
                addPrice(tmpPrice)
                count += 1
            End If

            ' note: if there is already some live data for this day, don't do min/max approximation

            ' min/max price approximation
            If close > open Then

                tmpPrice = New AssetPrice
                With tmpPrice
                    .ticker = asset.ticker
                    .price = low
                    .dat = step1Date
                    .todayChangePerc = (openPrice <> 0 AndAlso low / openPrice) Or 0
                End With
                tmpPrice.setCurrentMaxPrice(tmpMaxPrice)
                addPrice(tmpPrice)

                challengeTmpMaxPrice(high, step2Date)

                tmpPrice = New AssetPrice
                With tmpPrice
                    .ticker = asset.ticker
                    .price = high
                    .dat = step2Date
                    .todayChangePerc = (openPrice <> 0 AndAlso high / openPrice) Or 0
                End With
                tmpPrice.setCurrentMaxPrice(tmpMaxPrice)
                addPrice(tmpPrice)

            Else
                challengeTmpMaxPrice(high, step1Date)

                tmpPrice = New AssetPrice
                With tmpPrice
                    .ticker = asset.ticker
                    .price = high
                    .dat = step1Date
                    .todayChangePerc = (openPrice <> 0 AndAlso high / openPrice) Or 0
                End With
                tmpPrice.setCurrentMaxPrice(tmpMaxPrice)
                addPrice(tmpPrice)

                tmpPrice = New AssetPrice
                With tmpPrice
                    .ticker = asset.ticker
                    .price = low
                    .dat = step2Date
                    .todayChangePerc = (openPrice <> 0 AndAlso low / openPrice) Or 0
                End With
                tmpPrice.setCurrentMaxPrice(tmpMaxPrice)
                addPrice(tmpPrice)
            End If

            'close price
            tmpPrice = New AssetPrice
            With tmpPrice
                .ticker = asset.ticker
                .price = close
                .dat = closeDate
                .todayChangePerc = (openPrice <> 0 AndAlso close / openPrice) Or 0
            End With
            tmpPrice.setCurrentMaxPrice(tmpMaxPrice)
            addPrice(tmpPrice)

            count += 3

        Next
        Return count
    End Function

    Private Sub challengeTmpMaxPrice(price As Double, dat As Date)
        If price < tmpMaxPrice.price Then Exit Sub
        tmpMaxPrice.price = price
        tmpMaxPrice.dat = dat
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
                ' real-time enough
            Case UpdateSourceEnum.TRADING_VIEW
                ' opti: we could update all trading view assets at once
                TradingView.fetchPrice(asset)
                        ' 30 min delay
            Case UpdateSourceEnum.YAHOO
                        ' Yahoo.fetchPrice(asset)
            Case UpdateSourceEnum.BOURSOBANK

        ' we might get degiro realtime data, also removing yahoo 30 min delay usage
            Case UpdateSourceEnum.DEGIRO

        End Select

        ' hack for now
        If asset.ticker = FrmMain.mainGraph.asset.ticker Then
            FrmMain.mainGraph.render()
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

    Public Sub pushPriceToFile()

        ' 280403 162431|23.56
        ' 04/28/2024 1:50:00 PM|28.35

        Dim price As AssetPrice = currentPrice()
        If IsNothing(price) Then Exit Sub
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
        If maxPriceEver.dat.CompareTo(maxPrice.dat) < 0 Then maxPrice = maxPriceEver

        FrmMain.ToolStripProgressBarSimu.Maximum = prices.Count - 1
        FrmMain.ToolStripProgressBarSimu.Value = 0
    End Sub

    Public Function replayProgressIndex() As Integer
        Return replayIndex
    End Function

    Public Function replayProgress() As Double
        Return replayIndex / prices.Count
    End Function



    Public Function replayNext(Optional tick As Integer = 1) As Boolean
        ' thus we will never use the last value
        If replayIndex + tick > prices.Count - 1 Then Return False

        replayIndex += tick
        replayGoto(replayIndex)
        Return True
    End Function

    Public Sub replayGoto(index As Integer)
        replayIndex = index
        FrmMain.ToolStripProgressBarSimu.Value = replayIndex
    End Sub

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
