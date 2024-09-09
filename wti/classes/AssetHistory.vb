Imports System.IO

Public Class AssetHistory
    Public asset As AssetInfos

    Private prices As New List(Of AssetPrice)

    Private doingReplay As Boolean = False
    Private replayIndex As Integer

    Private maxPrice As AssetPrice
    Public diffWithMaxPerc As Double

    Public Sub New(asset As AssetInfos)
        Me.asset = asset

        If asset.persistHistory Then loadFromDataFromThePathFiles()

    End Sub

    Public Overrides Function ToString() As String
        Return StructToString(Me)
    End Function

    Public Sub loadFromDataFromThePathFiles()
        prices.Clear()

        For Each filePath As String In Directory.GetFiles(CST.DATA_PATH & "/dataFromThePast/")
            If Not filePath.Contains(asset.ticker) Then Continue For
            For Each line In File.ReadAllLines(filePath)
                addPrice(AssetPrice.Deserialize(asset, line))
            Next
        Next

        dbg.info("AssetHistory " & asset.ticker & " loaded " & prices.Count & " historical prices")

        prices.Sort(New AssetPriceDateComparer)
    End Sub

    Public Function currentPrice() As AssetPrice
        If doingReplay Then
            Return prices.ElementAt(replayIndex)
        Else
            Return prices.Last
        End If
    End Function

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


    Public Sub addPrice(ByRef price As AssetPrice)
        If IsNothing(maxPrice) OrElse price.price > maxPrice.price Then maxPrice = price
        prices.Add(price)
    End Sub

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


End Class



Public Class AssetPriceDateComparer
    Implements IComparer(Of AssetPrice)

    Public Function Compare(x As AssetPrice, y As AssetPrice) As Integer Implements IComparer(Of AssetPrice).Compare
        Return x.dat.CompareTo(y.dat)
    End Function
End Class