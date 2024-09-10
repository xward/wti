Imports System.IO

Public Module DegiroTransactionm

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

    Public Function cloneTransactionList(transactions As List(Of DegiroTransaction))
        Dim ret As New List(Of DegiroTransaction)
        For Each t In transactions
            ret.Add(t)
        Next
        Return ret
    End Function


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
End Module
