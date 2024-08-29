Imports System.IO

Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Namespace Ester
    Module Ester
        Public rate As Single = 4
        Dim url As String = "https://www.ecb.europa.eu/stats/financial_markets_and_interest_rates/euro_short-term_rate/html/index.en.html"

        Public Sub fetchRateFromBCE()
            Dim rep As String = Tesla.HttpGet(url)

            Try
                '<tr>
                '	<th><strong>Rate</strong></th>
                '	<td><strong>3.666</strong></td>
                '</tr> 

                Dim r As String = rep.Split("<th><strong>Rate</strong></th>
						<td><strong>").ElementAt(1).Split("</strong></td").ElementAt(0)

                rate = Single.Parse(r)
            Catch ex As Exception
                dbg.fail("Can' t parse ester rate")
            End Try

            dbg.info("Ester rate fetched: " & rate)
        End Sub
    End Module
End Namespace
