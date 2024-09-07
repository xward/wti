Public Class AssetPrice
    Public ticker As String
    Public price As Double
    Public todayChangePerc As Double
    Public dat As Date

    Public Sub New()
    End Sub

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
