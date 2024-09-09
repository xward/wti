Imports System.Security.Policy

Namespace Yahoo
    Module Yahoo
        'note: (?) can create an account to have live data

        Public Sub fetchPrice(asset As AssetInfos)
            Dim rep As String = Tesla.HttpGet(asset.yahooUrl)

            ' do stuff
        End Sub
    End Module

End Namespace

