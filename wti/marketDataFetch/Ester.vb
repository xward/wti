Imports System.IO

Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Namespace Ester
    Module Ester
        Public rate As Double = 13.37
        Dim url As String = "https://www.ecb.europa.eu/stats/financial_markets_and_interest_rates/euro_short-term_rate/html/index.en.html"

        Const FILE_PATH As String = "ester.txt"

        Public Sub fetchRateFromBCE()
            If alreadyDoneToday() Then
                rate = Double.Parse(File.ReadAllText(FILE_PATH))
                Exit Sub
            End If

            Dim rep As String = Tesla.HttpGet(url)

            Try
                '<tr>
                '	<th><strong>Rate</strong></th>
                '	<td><strong>3.666</strong></td>
                '</tr> 

                Dim r As String = rep.Split("<th><strong>Rate</strong></th>
						<td><strong>").ElementAt(1).Split("</strong></td").ElementAt(0)

                rate = Double.Parse(r)

                If Not alreadyDoneToday() Then
                    dbg.info("write ester to file")
                    File.WriteAllText(FILE_PATH, rate)
                End If

                dbg.info("Ester rate fetched: " & rate)

            Catch ex As Exception
                dbg.fail("Can' t parse ester rate")
            End Try

        End Sub

        Private Function alreadyDoneToday() As Boolean
            Dim d As Date = File.GetLastWriteTime(FILE_PATH)
            Return d.Year = Date.UtcNow.Year And d.Month = Date.UtcNow.Month And d.Day = Date.UtcNow.Day
        End Function
    End Module
End Namespace
