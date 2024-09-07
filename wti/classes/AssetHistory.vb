Imports System.DirectoryServices.ActiveDirectory
Imports System.IO

Public Class AssetHistory

    Public asset As AssetInfos

    Public prices As New List(Of AssetPrice)

    Private doingReplay As Boolean = False
    Private replayIndex As Integer


    Public Sub New(asset As AssetInfos)
        Me.asset = asset
        loadFromDataFromThePathFiles()
    End Sub

    Public Overrides Function ToString() As String
        Return StructToString(Me)
    End Function

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
        If doingReplay Then
            Return prices.ElementAt(replayIndex)
        Else
            Return prices.Last
        End If
    End Function

    ' todo: optional startDate
    Public Sub initReplay()
        doingReplay = True
        replayIndex = 0
    End Sub

    Public Function replayNext() As Boolean
        ' thus we will never use the last value
        If replayIndex = prices.Count - 1 Then Return False
        replayIndex += 1
        Return True
    End Function

End Class



Public Class AssetPriceDateComparer
    Implements IComparer(Of AssetPrice)

    Public Function Compare(x As AssetPrice, y As AssetPrice) As Integer Implements IComparer(Of AssetPrice).Compare
        Return y.dat.CompareTo(x.dat)
    End Function
End Class