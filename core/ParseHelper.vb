Module ParseHelper

    Public Function parseMoney(s As String) As Double

        If s = "—" Then Return Nothing

        ' test with more than 1000
        ' avoid 1.123,423 being changed to 1.123.423

        Dim cleaned As String = s.Replace("−", "-").Replace(",", ".").Replace("€", "").Trim
        Dim val As Double
        If Double.TryParse(cleaned, val) Then
            Return val
        Else
            dbg.fail("fail to parse money number >" & s & "<")
        End If
        Return Nothing
    End Function
End Module
