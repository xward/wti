Namespace SP500

    Module SP500
        ' what we have as advantage:
        ' - below or above global trend
        ' - ca monte toujours, globalement
        ' - si ca descend ca va remonter, on sait juste pas quand, et plus on est loin du max ever plus ca sera fort
        ' - en cas de fuckup ca reviendra, mais peut etre après un long moment


        ' detect major crise: 45% value loss in X temps

        ' fetch/update historique data long game

        ' 3. ecart avec trend
        ' 2. stable depuis 2h?
        ' 1. ca vient de chuter de x% en y temps

        ' ----------------------------------------------------------------------------------------------------------------------------------------------
        ' sp500 spx poc


        Private sp500 As AssetInfos = assetInfo(AssetNameEnum.SP500)

        Public Sub runPocSpx()
            status = StatusEnum.SIMU
            dbg.info("STARTING SIMULATION")
            Degiro.SIMU_init()
            FrmMain.ToolStripProgressBarSimu.Visible = True
            ' todo: from date to date
            replayInit(sp500)

            ' Degiro.SIMU_placeOrUpdateOrder(sp500.ticker, 5, "Achat", 88, Nothing)

            ' diff from max iz pt, pas de maxprice ?

            While status = StatusEnum.SIMU And replayNext(sp500)


                Dim price As AssetPrice = getPrice(sp500)

                If Degiro.orders.Count = 0 And Degiro.positions.Count = 0 And price.diffFromMaxPrice > 8 Then
                    Degiro.SIMU_placeOrUpdateOrder(sp500.ticker, 5, "Achat", price.price, Nothing)
                End If

                ' sell if better
                If Degiro.orders.Count = 0 And Degiro.positions.Count = 1 And price.diffFromMaxPrice < 4 Then
                    Degiro.SIMU_placeOrUpdateOrder(sp500.ticker, 5, "Vente", price.price, Nothing)
                End If



                Degiro.SIMU_updateAll()
                Degiro.updateTradePanelUI()

            End While


            dbg.info(Degiro.transactions.Count)

            dbg.info("simu completed")
            status = StatusEnum.OFFLINE
            FrmMain.ToolStripProgressBarSimu.Visible = False
        End Sub












        ' ----------------------------------------------------------------------------------------------------------------------------------------------
        ' 3x v0

        Private sp5003x As AssetInfos = assetInfo(AssetNameEnum.SP500_3X)
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
