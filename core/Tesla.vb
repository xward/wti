Imports System.IO
Imports System.Net.Http

Imports System.Threading.Tasks


Namespace Tesla
    Module Tesla

        Dim client As New System.Net.Http.HttpClient

        Public Function HttpGet(url As String) As String

            Dim task As Task(Of HttpResponseMessage) = task.Run(Function() client.GetAsync(url))
            task.Wait()
            Dim rep As HttpResponseMessage = task.Result

            If rep.StatusCode = Net.HttpStatusCode.OK Then
                Dim reader As New StreamReader(rep.Content.ReadAsStream)
                Dim streamText As String = reader.ReadToEnd()
                '  dbgMain.info(streamText)
                Return streamText
            Else
                dbg.fail("Fail to fetch " & url & " status code: " & rep.StatusCode.ToString)

                Return Nothing
            End If
        End Function
    End Module
End Namespace
