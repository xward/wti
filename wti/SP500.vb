﻿Module SP500
    ' what we have as advantage:
    ' - below or above global trend
    ' - ca monte toujours, globalement
    ' - si ca descend va ca remonter, on sait juste pas quand
    ' - en cas de fuckup ca reviendra, mais peut etre après un long moment



    ' detect major crise: 45% value loss in X temps

    ' fetch/update historique data long game

    ' ecart avec trend
    ' stable depuis 2h?
    ' ca vient de chuter


    Public Sub simulateStupidAlgo()
        FrmMain.SIMU_MODE = True

        dbg.info("STARTIN SIMULATION")
        Degiro.SIMU_init()

        Dim asset As AssetInfos = assetInfo("3USL")

        Degiro.SIMU_placeOrUpdateOrder(asset.ticker, 5, "Achat", 88, Nothing)


        status = StatusEnum.SIMU

        Dim resoldOrder As Boolean = False

        While status = StatusEnum.SIMU And TradingView.SIMU_setNext(asset)

            ''' DECISION
            ''' 
            Dim price As AssetPrice = TradingView.getPrice(asset)

            'If Degiro.accountCashMoula > 5 * 86 Then
            '    Degiro.SIMU_placeOrUpdateOrder(asset.ticker, 5, "Achat", 86, Nothing)
            'End If

            If Degiro.orders.Count = 0 And Not (resoldOrder) Then
                resoldOrder = True
                Degiro.SIMU_placeOrUpdateOrder(asset.ticker, 5, "Vente", 89, Nothing)
            End If


            Pause(5)

            ''' 
            Degiro.SIMU_updateAll()
            Degiro.updateTradePanelUI()
        End While


        dbg.info(Degiro.transactions.Count)

        dbg.info("simu completed")
        status = StatusEnum.OFFLINE
        FrmMain.SIMU_MODE = False
    End Sub

End Module