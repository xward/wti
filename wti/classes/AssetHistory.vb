Imports System.DirectoryServices.ActiveDirectory
Imports System.IO

Public Class AssetHistory

    Public asset As AssetInfos

    Public prices As New List(Of AssetPrice)


    Public Sub New(asset As AssetInfos)
        Me.asset = asset
    End Sub

    Public Sub loadFromDataFromThePathFiles()
        prices.Clear()

        For Each filePath As String In Directory.GetFiles(CST.DATA_PATH & "/dataFromThePast/")
            If Not filePath.Contains(asset.ticker) Then Continue For
            For Each line In File.ReadAllLines(filePath)
                prices.Add(AssetPrice.Deserialize(asset, line))
            Next
        Next

        prices.Sort(New AssetPriceDateComparer)
    End Sub

    Public Function currentPrice() As AssetPrice
        Return prices.Last
    End Function

End Class



Public Class AssetPriceDateComparer
    Implements IComparer(Of AssetPrice)

    Public Function Compare(x As AssetPrice, y As AssetPrice) As Integer Implements IComparer(Of AssetPrice).Compare
        Return y.dat.CompareTo(x.dat)
    End Function
End Class