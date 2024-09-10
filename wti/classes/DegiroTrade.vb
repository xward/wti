Imports System.IO

Public Module DegiroTradem

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

    Public Function cloneTradeList(transactions As List(Of DegiroTrade))
        Dim ret As New List(Of DegiroTrade)
        For Each t In transactions
            ret.Add(t)
        Next
        Return ret
    End Function

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

End Module
