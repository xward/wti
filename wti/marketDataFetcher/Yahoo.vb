Imports System.Security.Policy

Namespace Yahoo
    Module Yahoo

        'this module is useless, can't use yahoo to fetch live enough data
        'we use yahoo in python for past data

        ' or ..

        ' we could use yahoo to fetch late 30min data per minute, we would have to call python script for that


        'note: (?) can create an account to have live data

        Public Sub fetchPrice(asset As AssetInfos)
'load both long data and live data, if any


            'Dim rep As String = Tesla.HttpGet(asset.yahooUrl)


            ' do stuff

            'dbg.info(rep)
            'dbg.info(asset.yahooUrl)
        End Sub

        private pyFetchOneLiveFilePath as string = cst.ROOT_PATH & "/pythonlab/yahoo_fetch_one_live.py"

        public sub pyFetch(asset as AssetInfos)
        Dim start_info As New ProcessStartInfo("ouaiouai", pyFetchOneLiveFilePath & " " & asset.yahooTicker)
        start_info.UseShellExecute = False
        start_info.CreateNoWindow = True
        start_info.RedirectStandardOutput = True
        start_info.RedirectStandardError = True

        ' Make the process and set its start information.
        Dim proc As New Process()
        proc.StartInfo = start_info

        Dim dt As Date = Now()

        ' Start the process.
        proc.Start()

         ' Attach to stdout and stderr.
        Dim std_out As StreamReader = proc.StandardOutput() ' will not continue until process stops
        Dim std_err As StreamReader = proc.StandardError()

        ' Retrive the results.
        Dim sOut As String = std_out.ReadToEnd()
        Dim sErr As String = std_err.ReadToEnd()

        dbg.info(sOut)
        dbg.info(sErr)

        end sub
    End Module

End Namespace
