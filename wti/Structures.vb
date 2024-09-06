Imports System.IO
Imports System.Reflection

Module Structures
    'please take care  to sell trade with this strat

    Public status As StatusEnum = StatusEnum.OFFLINE

    Public Enum StatusEnum
        OFFLINE
        ONLINE
        SIMU
        LIVE
        'just collect data from trading view
        COLLECT
    End Enum

    ' ----------------------------------------------------------------------------------------
    Public Function cloneTransactionList(transactions As List(Of DegiroTransaction))
        Dim ret As New List(Of DegiroTransaction)
        For Each t In transactions
            ret.Add(t)
        Next
        Return ret
    End Function

    Public Function cloneTradeList(transactions As List(Of DegiroTrade))
        Dim ret As New List(Of DegiroTrade)
        For Each t In transactions
            ret.Add(t)
        Next
        Return ret
    End Function


    'date ticker isin placeBoursiere action qté limitPrix€ 33,00 stop(€ —) valeur ouvert execution

    Public Structure DegiroOrder
        'unused
        Dim id As String

        ' 3OIL
        Dim ticker As String
        ' IE00BMTM6B32
        Dim isin As String

        Dim dat As Date

        ' Vente Achat
        Dim orderAction As String

        Dim quantity As Integer

        Dim limit As Double
        Dim stopPrice As Double
    End Structure

    Public Structure DegiroPosition
        'unused
        Dim id As String
        ' 3OIL
        Dim ticker As String
        ' IE00BMTM6B32
        Dim isin As String

        Dim quantity As Integer
        'how much is it now
        Dim currentTotalValue As Double
        'how much you bought it
        Dim pru As Double

    End Structure

    Public Structure DegiroTransaction
        'unused
        Dim id As String
        ' 3OIL
        Dim ticker As String
        ' IE00BMTM6B32
        Dim isin As String

        Dim dat As Date

        ' Achat | Vente
        Dim action As String

        Dim quantity As Integer
        'in case of Achat, amount that as been sold in another transaction
        Dim quantityFragmentSold As Integer
        'how much you bought/sell it
        Dim pru As Double

        Dim fee As Double

    End Structure


    Public Function transactionsFromFiles() As List(Of DegiroTransaction)
        Dim l As New List(Of DegiroTransaction)

        For Each filePath As String In Directory.GetFiles(CST.DATA_PATH & "/degiroTransactions/")
            'If filePath.Contains("attachedToTrade") Then Continue For
            Dim t As DegiroTransaction = deserializeTransaction(File.ReadAllText(filePath))
            ' dbg.info("Load transaction from file >> " & StructToString(t))
            l.Add(t)
        Next
        dbg.info("Loaded " & l.Count & " transactions from file")
        Return l
    End Function

    Public Function completedTransactionToFilePath(t As DegiroTransaction) As String
        Return CST.DATA_PATH & "/degiroTransactions/attachedToTrade/" & dateToPrettySortableString(t.dat) & " " &
            t.ticker & " " & t.action & " quantity=" & t.quantity & " pru=" & t.pru & "€.transaction.degiro.txt"
    End Function

    Public Function transactionToFilePath(t As DegiroTransaction) As String
        Return CST.DATA_PATH & "/degiroTransactions/" & dateToPrettySortableString(t.dat) & " " &
            t.ticker & " " & t.action & " quantity=" & t.quantity & " pru=" & t.pru & "€.transaction.degiro.txt"
    End Function

    Public Function serializeTransaction(t As DegiroTransaction) As String
        Return t.ticker & "|" & t.isin & "|" & t.dat.ToString & "|" & t.action & "|" & t.quantity & "|" & t.quantityFragmentSold & "|" & t.pru & "|" & t.fee
    End Function

    Public Function deserializeTransaction(s As String) As DegiroTransaction
        Dim split As String() = s.Split("|")

        ' CSCO|US17275R1023|8/18/2020 4:10:07 PM|Achat|1|0|35.17|-0.5

        Return New DegiroTransaction With {
            .ticker = split.ElementAt(0),
            .isin = split.ElementAt(1),
            .dat = Date.Parse(split.ElementAt(2)),
            .action = split.ElementAt(3),
            .quantity = Math.Abs(Integer.Parse(split.ElementAt(4))),
            .quantityFragmentSold = Integer.Parse(split.ElementAt(5)),
            .pru = Double.Parse(split.ElementAt(6)),
            .fee = Double.Parse(split.ElementAt(7))
            }
    End Function

    ' the complete buy/sell tracking object
    '' currently bs filling
    Public Structure DegiroTrade
        ' id

        ' ME, X

        Dim ticker As String
        Dim isin As String

        ' BUYING

        ' BUY DONE
        Dim buyDone As Boolean
        Dim buyFee As Double
        Dim buyDate As Date

        Dim quantity As Integer
        Dim pru As Double

        ' SELLING
        Dim sellPricePerUnit As Double
        Dim perfPerc As Double
        Dim totalPlusValue As Double

        ' SELL DONE
        Dim sellDone As Boolean
        Dim sellFee As Double
        Dim sellDate As Date
    End Structure


    Public Function tradesFromFiles() As List(Of DegiroTrade)
        Dim l As New List(Of DegiroTrade)

        For Each filePath As String In Directory.GetFiles(CST.DATA_PATH & "/degiroTrades/")
            Dim t As DegiroTrade = deserializeTrade(File.ReadAllText(filePath))
            ' dbg.info("Load trade from file >> " & StructToString(t))
            l.Add(t)
        Next
        dbg.info("Loaded " & l.Count & " trades from file")
        Return l
    End Function

    Public Function tradeToFilePath(t As DegiroTrade) As String
        Return CST.DATA_PATH & "/degiroTrades/" & dateToPrettySortableString(t.sellDate) &
            t.ticker & " quantity=" & t.quantity & " pru=" & t.pru & "€.transaction.degiro.txt"
    End Function

    Public Function serializeTrade(t As DegiroTrade) As String
        Return t.ticker & "|" & t.isin & "|" & t.buyDone & "|" & t.buyFee & "|" & t.buyDate.ToString & "|" & t.quantity & "|" & t.pru & "|" & t.sellPricePerUnit & "|" & t.perfPerc & "|" & t.totalPlusValue & "|" & t.sellDone & "|" & t.sellFee & "|" & t.sellDate.ToString
    End Function

    Public Function deserializeTrade(s As String) As DegiroTrade
        Dim split As String() = s.Split("|")

        Return New DegiroTrade With {
            .ticker = split.ElementAt(0),
            .isin = split.ElementAt(1),
            .buyDone = Boolean.Parse(split.ElementAt(2)),
            .buyFee = Double.Parse(split.ElementAt(3)),
            .buyDate = Date.Parse(split.ElementAt(4)),
            .quantity = Integer.Parse(split.ElementAt(5)),
            .pru = Double.Parse(split.ElementAt(6)),
            .sellPricePerUnit = Double.Parse(split.ElementAt(7)),
            .perfPerc = Double.Parse(split.ElementAt(8)),
            .totalPlusValue = Double.Parse(split.ElementAt(9)),
            .sellDone = Boolean.Parse(split.ElementAt(10)),
            .sellFee = Double.Parse(split.ElementAt(11)),
            .sellDate = Date.Parse(split.ElementAt(12))
            }
    End Function


    Public Class AssetPrice
        Public ticker As String
        Public price As Double
        Public todayChangePerc As Double
        Public dat As Date

        Public Sub New()

        End Sub

        'Public Sub New(asset As AssetInfos, s As String)
        '    Deserialize(asset, s)
        'End Sub

        Public Overrides Function ToString() As String
            Return StructToString(Me)
        End Function

        Public Function Serialize() As String
            Return Me.dat.Day.ToString("00") & Me.dat.Month.ToString("00") & Me.dat.Year.ToString.Substring(2) & " " &
           Me.dat.Hour.ToString("00") & Me.dat.Minute.ToString("00") & Me.dat.Second.ToString("00") & "|" & Me.price & "|" & Me.todayChangePerc
        End Function

        Public Shared Function Deserialize(asset As AssetInfos, s As String) As AssetPrice
            Dim split As String() = s.Split("|")

            Dim datChunk As String = split.ElementAt(0)

            ' dbg.info(datChunk)
            Dim dat As Date = Date.Parse(datChunk.Substring(2, 2) & "/" & datChunk.Substring(0, 2) & "/20" & datChunk.Substring(4, 2) & " " & datChunk.Substring(7, 2) & ":" & datChunk.Substring(9, 2) & ":" & datChunk.Substring(11, 2))

            Return New AssetPrice With {
                .ticker = asset.ticker,
                .price = Double.Parse(split.ElementAt(1)),
                .dat = dat,
                .todayChangePerc = ((split.Count = 3) AndAlso Double.Parse(split.ElementAt(2))) Or 0
            }
        End Function
    End Class


    Public Structure AssetInfos
        ' if etf
        Dim ISIN As String
        'surname to disaplay
        Dim name As String
        Dim fullName As String

        Dim tradingViewUrl As String
        ' also called ticker
        Dim ticker As String
        Dim leverage As Integer
        Dim isShort As Boolean
        ' if apply, only for raw like wti but not it's ETF version w or wo leverage
        Dim futurUrl As String
        Dim degiroOrderUrl As String
        Dim degireId As Integer

        Dim marketOpen As Date
        Dim marketUTCClose As Date
    End Structure


    Public Function assetInfo(assetTicker As String) As AssetInfos
        Select Case assetTicker
            Case "3OIL"
                Return New AssetInfos With {
                    .ISIN = "IE00BMTM6B32",
                    .name = "WTI 3X",
                    .fullName = "WISDOMTREE WTI CRUDE OIL 3X DAILY L",
                    .tradingViewUrl = "https://www.tradingview.com/chart/aSPkAHjR/?symbol=MIL%3A3OIL",
                    .ticker = "3OIL",
                    .leverage = 3,
                    .isShort = False,
                    .degiroOrderUrl = "https://trader.degiro.nl/trader/?appMode=order#/markets?newOrder&action=buy&productId=18744180",
                    .degireId = 18744180
                }
            Case "3OIS"
                Dim tradingViewUrl As String = ""
                Select Case CST.HOST_NAME
                    Case CST.CST.hostNameEnum.GALACTICA
                        tradingViewUrl = "https://www.tradingview.com/chart/2OrM2Knv/?symbol=MIL%3A3OIS"
                    Case CST.CST.hostNameEnum.GHOST
                        tradingViewUrl = "https://www.tradingview.com/chart/vjhxMR0Z/?symbol=MIL%3A3OIS"
                End Select

                ' https://www.tradingview.com/chart/vjhxMR0Z/?symbol=MIL%3A3OIS
                Return New AssetInfos With {
                    .ISIN = "XS2819844387",
                    .name = "WTI 3X Short",
                    .fullName = "WISDOMTREE WTI CRUDE OIL 3X DAILY SHO",
                    .tradingViewUrl = tradingViewUrl,
                    .ticker = "3OIS",
                    .leverage = 3,
                    .isShort = True,
                    .degiroOrderUrl = "https://trader.degiro.nl/trader/?appMode=order#/markets?newOrder&action=buy&productId=30311482",
                    .degireId = 30311482
                }

            Case "3USL"
                Dim tradingViewUrl As String = ""
                Select Case CST.HOST_NAME
                    Case CST.CST.hostNameEnum.GALACTICA
                        tradingViewUrl = "https://www.tradingview.com/chart/2OrM2Knv/?symbol=MIL%3A3USL"
                    Case CST.CST.hostNameEnum.GHOST
                        tradingViewUrl = "https://www.tradingview.com/chart/vjhxMR0Z/?symbol=MIL%3A3USL"
                End Select

                Return New AssetInfos With {
                    .ISIN = "IE00B7Y34M31",
                    .name = "SP500 3X",
                    .fullName = "WISDOMTREE S&P 500 3X DAILY LEVERAG",
                    .tradingViewUrl = tradingViewUrl,
                    .ticker = "3USL",
                    .leverage = 3,
                    .isShort = False,
                    .degiroOrderUrl = "https://trader.degiro.nl/trader/?appMode=order#/markets?newOrder&action=buy&productId=4995112",
                    .degireId = 4995112,
                    .marketOpen = Date.Parse("01/01/2024 07:00"),
                    .marketUTCClose = Date.Parse("01/01/2024 15:45")
                }
        End Select
        Return New AssetInfos With {.ticker = "nothing"}
    End Function

    Public Enum OrderTypeEnum
        LIMIT_SELL
        LIMIT_BUY
        ' stop loss
        STOP_SELL
        STOP_BUY
    End Enum

    Public Function StructToString(obj As Object) As String
        Dim structString As String = ""
        Dim i As Integer
        Dim myType As Type = obj.GetType()
        Dim myField As FieldInfo() = myType.GetFields()
        For i = 0 To myField.Length - 1
            structString &= myField(i).Name & ":" & myField(i).GetValue(obj) & " "
        Next i

        Return structString
    End Function

    'Public Function degiroOrderToString(o As DegiroOrder) As String
    '    Return o.ticker & " " & o.isin & " " & o.dat.ToString & " " & o.orderAction & " " & o.quantity & " " & o.limit & " " & o.stopPrice
    'End Function


End Module
