﻿Public Class AssetPrice
    Public ticker As String
    Public price As Double
    Public todayChangePerc As Double
    Public dat As Date

    ' virtual fields, omit from serializing
    ' goal: indicators
    Public currentMaxPrice As Double = 0
    Public currentMaxPriceDate As Date = Date.UtcNow


    Public stability100min As AssetStability

    Public Overrides Function ToString() As String
        Return ticker & " " & dat.ToString & " " & price & " (" & todayChangePerc & ")"
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

    ' rounded diff from max price perc
    'return number between 0.00 and 100.00
    Public Function diffFromMaxPrice() As Double
        If currentMaxPrice = 0 Then Return 0
        Return Math.Round((1 - price / currentMaxPrice) * 100 * 100) / 100
    End Function

    Public Sub setCurrentMaxPrice(maxPrice As AssetPrice)
        currentMaxPrice = maxPrice.price
        currentMaxPriceDate = maxPrice.dat
    End Sub
End Class

Public Structure AssetStability
    Dim min As Double
    Dim max As Double
    Dim value As Double
End Structure