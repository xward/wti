Imports System.Security.Policy

Namespace Yahoo
    Module Yahoo

        'this module is useless, can't use yahoo to fetch live enough data
        'we use yahoo in python for past data

        ' or ..

        ' we could use yahoo to fetch late 30min data per minute, we would have to call python script for that


        'note: (?) can create an account to have live data

        Public Sub fetchPrice(asset As AssetInfos)
            'Dim rep As String = Tesla.HttpGet(asset.yahooUrl)


            '' do stuff

            'dbg.info(rep)
            'dbg.info(asset.yahooUrl)
        End Sub
    End Module

End Namespace

