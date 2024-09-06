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

    Public Function splitTableLine(l As String) As List(Of String)
        l = l.Replace("$", "").Replace("€", "").Trim
        Dim tmpSplit As String() = l.Split({" ", "	"}, StringSplitOptions.None)
        Dim split As New List(Of String)
        For Each s As String In tmpSplit
            If s.Trim.Length > 0 Then split.Add(s)
        Next
        Return split
    End Function

    Public Sub printListOfString(l As List(Of String))
        dbg.info("elem count= " & l.Count & " " & String.Join(" ", l))
    End Sub

End Module
