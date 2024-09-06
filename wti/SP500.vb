Module SP500
    ' what we have as advantage:
    ' - below or above global trend
    ' - ca monte toujours, globalement
    ' - si ca descend va ca remonter, on sait juste pas quand
    ' - en cas de fuckup ca reviendra, mais peut etre après un long moment



    ' detect major crise: 45% value loss in X temps

    ' fetch/update historique data long game




    Public Sub simulateStupidAlgo()
        Dim asset As AssetInfos = assetInfo("3USL")


        status = StatusEnum.SIMU

        While status = StatusEnum.SIMU And TradingView.SIMU_setNext(asset)

            ''' DECISION


            ''' 
            Degiro.SIMU_updateAll()
        End While

        ' repeat !
    End Sub

End Module
