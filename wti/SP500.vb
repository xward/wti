Namespace SP500

    Module SP500
        ' what we have as advantage:
        ' - below or above global trend
        ' - ca monte toujours, globalement
        ' - si ca descend va ca remonter, on sait juste pas quand
        ' - en cas de fuckup ca reviendra, mais peut etre après un long moment

        Public Sub init()
            ' load value from csv and push to assetHistory of sp500


        End Sub



        ' detect major crise: 45% value loss in X temps

        ' fetch/update historique data long game

        ' 3. ecart avec trend
        ' 2. stable depuis 2h?
        ' 1. ca vient de chuter de x% en y temps


        ' fake degiro lack: a bit of refacto

        Public sp5003x As AssetInfos = assetInfo("3USL")
        Public Sub simulateStupidAlgo()
            status = StatusEnum.SIMU
            dbg.info("STARTING SIMULATION")

            Degiro.SIMU_init()

            replayInit(sp5003x)

            Degiro.SIMU_placeOrUpdateOrder(sp5003x.ticker, 5, "Achat", 88, Nothing)


            Dim resoldOrder As Boolean = False

            While status = StatusEnum.SIMU And replayNext(sp5003x)
                FrmMain.bottomGraph.render()

                ''' DECISION
                ''' 
                Dim price As AssetPrice = getPrice(sp5003x)

                'If Degiro.accountCashMoula > 5 * 86 Then
                '    Degiro.SIMU_placeOrUpdateOrder(asset.ticker, 5, "Achat", 86, Nothing)
                'End If

                If Degiro.orders.Count = 0 And Not (resoldOrder) Then
                    resoldOrder = True
                    Degiro.SIMU_placeOrUpdateOrder(sp5003x.ticker, 5, "Vente", 89, Nothing)
                End If


                ''' 
                Degiro.SIMU_updateAll()
                Degiro.updateTradePanelUI()

            End While


            dbg.info(Degiro.transactions.Count)

            dbg.info("simu completed")
            status = StatusEnum.OFFLINE
        End Sub

    End Module

End Namespace
