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

        ' load prices
        prices.Clear()
        'if any, load it
        loadDataFromPersistHistoryFromLiveCollect()
        if populateDataFromYahooMinute then loadDataFromYahooMinute()

if populateDataFromYahooDaily then loadDataFromYahooDaily()


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

' to make suuure data engine is not down and we place an order for that
    public function lastUpdateAgoSec() as Double
return Date.UtcNow().Subtract(lastDataSourceUpdate).TotalSeconds
end function

    ' only MarketPrice or Me can call me
    Public Sub addPrice(ByRef price As AssetPrice)
        If IsNothing(maxPrice) OrElse price.price > maxPrice.price Then maxPrice = price
        If IsNothing(maxPriceEver) OrElse price.price > maxPriceEver.price Then
            maxPriceEver = price
            pushMaxEverFile()
        End If

        lastDataSourceUpdate = Date.UtcNow()

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

    Private Sub loadDataFromPersistHistoryFromLiveCollect()
    dim count as integer = 0
        For Each filePath As String In Directory.GetFiles(CST.DATA_PATH & "/dataFromThePast/")
            If Not filePath.Contains(asset.ticker) Then Continue For
            If Not filePath.Contains(".tv.txt") Then Continue For
            For Each line In File.ReadAllLines(filePath)
                addPrice(AssetPrice.Deserialize(asset, line))
                count += 1
            Next
        Next

        dbg.info("AssetHistory " & asset.ticker & " loaded " & count & " historical prices from live collect")
    End Sub

    private sub loadDataFromYahooMinute()
    dim count as integer = 0
    For Each filePath As String In Directory.GetFiles(CST.DATA_PATH & "/yahoo/")
            If Not filePath.Contains(asset.yahooTicker) Then Continue For
            If Not filePath.Contains(".minute.yahoo.txt") Then Continue For
            For Each line In File.ReadAllLines(filePath)
                count += loadFromAYahooFile(filePath)
            Next
        Next
 dbg.info("AssetHistory " & asset.ticker & " loaded " & count & " prices from yahoo minute history")
    end sub

    Private Sub loadDataFromYahooDaily()
        dim filePath as String = CST.DATA_PATH & "/yahoo/" & asset.yahooTicker & ".daily.yahoo.txt"
        dim count as integer = 0
        if File.Exists(filePath) do
            count += loadFromAYahooFile(filePath)

            dbg.info("AssetHistory " & asset.ticker & " loaded " & count & " prices from yahoo daily history")
        Else
            dbg.fail("[" & asset.ticker  & "] No daily yahoo file found")
        end if
    End Sub


    private function loadFromAYahooFile(filePath as string) as integer
    dim daily as boolean = false
    dim count as integer
    dim openPrice as double = -1
     For Each line In File.ReadAllLines(filePath)
        if line.Contains("Date;") then
        daily= true
         continue for
end if
if line.Contains("Datetime;") then continue for
        dim split as String() = line.Split(";")

        ' daily
        ' Date;Open;High;Low;Close;Adj Close;Volume
        ' 1970-01-02;0.0;93.54000091552734;91.79000091552734;93.0;93.0;8050000
        ' minute
        ' Datetime;Open;High;Low;Close;Adj Close;Volume
        ' 2024-09-09 09:30:00-04:00;5442.06982421875;5450.18994140625;5442.06982421875;5448.89013671875;5448.89013671875;0


      Dim datChunk As String = split.ElementAt(0)

      dim openDate as Date
      dim closeDate as date
      dim step1Date as date
      dim step2Date as date

      if daily then
      openDate = Date.parse(datChunk & " 09:30:00")
      closeDate = Date.parse(datChunk & " 16:00:00")
      step1Date = openDate.AddHours(2)
      step2Date = openDate.AddHours(4)
      Else
        openDate = Date.parse(datChunk )
      closeDate = openDate.addSeconds(59)
      step1Date = openDate.addSeconds(20)
      step2Date = openDate.addSeconds(40)
      end

dim open as double = Double.parse(split.ElementAt(1))
dim high as double = Double.parse(split.ElementAt(2))
dim low as double = Double.parse(split.ElementAt(3))
dim close as double = Double.parse(split.ElementAt(4))

if openPrice = -1 then openPrice = open

   count += 4
' open price
addPrice(New AssetPrice With {
            .ticker = asset.ticker,
            .price = open,
            .dat = openDate,
            .todayChangePerc =  (openPrice <> 0 && open / openPrice) || 0
        })
'close price
        addPrice(New AssetPrice With {
            .ticker = asset.ticker,
            .price = close,
            .dat = closeDate,
            .todayChangePerc = (openPrice <> 0 && close / openPrice) || 0
        })

' note: if there is already some live data for this day, don't do min/max approximation

' min/max price approximation
if close > open then
addPrice(New AssetPrice With {
            .ticker = asset.ticker,
            .price = low,
            .dat = step1Date,
            .todayChangePerc = (openPrice <> 0 && low / openPrice) || 0
        })
        addPrice(New AssetPrice With {
            .ticker = asset.ticker,
            .price = high,
            .dat = step2Date,
                  .todayChangePerc = (openPrice <> 0 && high / openPrice) || 0
        })
Else
addPrice(New AssetPrice With {
            .ticker = asset.ticker,
            .price = high,
            .dat = step1Date,
                  .todayChangePerc = (openPrice <> 0 && high / openPrice) || 0
        })
        addPrice(New AssetPrice With {
            .ticker = asset.ticker,
            .price = low,
            .dat = step2Date,
                    .todayChangePerc = (openPrice <> 0 && low / openPrice) || 0
        })
        end if
  Next
  return count
    end function

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

    public Sub pushPriceToFile()

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
