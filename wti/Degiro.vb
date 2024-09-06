Imports System.IO
Imports System.Reflection
Imports System.Runtime

Namespace Degiro
    Module Degiro
        ' Total Compte
        Public accountTotalMoula As Double
        ' Portefeuille
        Public accountPositionsMoula As Double
        ' EUR Cash tradable
        Public accountCashMoula As Double
        ' P/L Cumulée
        Public accountWinLooseMoula As Double

        Public orders As New List(Of DegiroOrder)
        Public positions As New List(Of DegiroPosition)
        ' stash transactions stored in file
        Private previousTransactions As New List(Of DegiroTransaction)
        ' final aggreation or transactions that doesn't belong to a completed trade
        Public transactions As New List(Of DegiroTransaction)
        ' stash trades stored in file
        Private previousTrades As New List(Of DegiroTrade)
        ' final aggreation 
        Public trades As New List(Of DegiroTrade)

        'todo: display it
        Public lastUpdate As Date


        Private SIMU_spread As Double = 0.015
        Private SIMU_fee As Double = 3

        Public Sub SIMU_init()
            orders.Clear()
            positions.Clear()
            transactions.Clear()
            previousTransactions.Clear()
            trades.Clear()
            previousTrades.Clear()

            accountTotalMoula = 10000
            accountPositionsMoula = 0
            accountCashMoula = 10000
            accountWinLooseMoula = 0

            TradingView.SIMU_init(assetInfo("3USL"))


            dbg.info("SIMU: intialized !")
        End Sub

        Public Sub SIMU_updateAll()
            Dim executedOrders As New List(Of DegiroOrder)

            ' accountPositionsMoula marche pas, il faudrait l' update =f(positions)


            For Each o As DegiroOrder In orders
                Dim price As AssetPrice = TradingView.getPrice(assetInfo(o.ticker))

                If price.price = 0 Then dbg.fail("SIMU: No price found for " & o.ticker)

                Select Case o.orderAction
                    Case "Vente"
                        If o.limit <> 0 And price.price > o.limit Then
                            accountTotalMoula += o.limit * o.quantity - SIMU_fee
                            accountPositionsMoula -= o.limit * o.quantity
                            accountCashMoula += o.limit * o.quantity - SIMU_fee

                            executedOrders.Add(o)

                            Dim t As DegiroTransaction = New DegiroTransaction With {
                                .ticker = o.ticker,
                                .action = "Vente",
                                .quantity = o.quantity,
                                .dat = Date.UtcNow,
                                .fee = SIMU_fee,
                                .isin = assetInfo(o.ticker).ISIN,
                                .pru = o.limit
                            }

                            transactions.Add(t)
                            dbg.info(" New transaction " & StructToString(t))


                            positions.Clear()

                        ElseIf o.stopPrice <> 0 And price.price < o.stopPrice Then
                            accountTotalMoula += o.stopPrice * o.quantity - SIMU_fee
                            accountPositionsMoula -= o.stopPrice * o.quantity
                            accountCashMoula += o.stopPrice * o.quantity - SIMU_fee

                            executedOrders.Add(o)

                            Dim t As DegiroTransaction = New DegiroTransaction With {
                                .ticker = o.ticker,
                                .action = "Vente",
                                .quantity = o.quantity,
                                .dat = Date.UtcNow,
                                .fee = SIMU_fee,
                                .isin = assetInfo(o.ticker).ISIN,
                                .pru = o.stopPrice
                            }

                            transactions.Add(t)
                            dbg.info(" New transaction " & StructToString(t))

                            positions.Clear()

                        End If

                    Case "Achat"
                        If o.limit <> 0 And price.price < o.limit Then
                            accountTotalMoula -= o.limit * o.quantity * (1 + SIMU_spread) + SIMU_fee
                            accountPositionsMoula += o.limit * o.quantity
                            accountCashMoula -= o.limit * o.quantity * (1 + SIMU_spread) + SIMU_fee

                            executedOrders.Add(o)

                            Dim t As DegiroTransaction = New DegiroTransaction With {
                                .ticker = o.ticker,
                                .action = "Achat",
                                .quantity = o.quantity,
                                .dat = Date.UtcNow,
                                .fee = SIMU_fee,
                                .isin = assetInfo(o.ticker).ISIN,
                                .pru = o.limit * (1 + SIMU_spread)
                            }

                            transactions.Add(t)
                            dbg.info(" New transaction " & StructToString(t))


                            positions.Add(New DegiroPosition With {
                               .ticker = o.ticker,
                               .pru = o.limit,
                               .quantity = o.quantity,
                               .currentTotalValue = o.limit * o.quantity
                               })

                        ElseIf o.stopPrice <> 0 And price.price > o.stopPrice Then
                            accountTotalMoula -= o.stopPrice * o.quantity * (1 + SIMU_spread) + SIMU_fee
                            accountPositionsMoula += o.stopPrice * o.quantity
                            accountCashMoula -= o.stopPrice * o.quantity * (1 + SIMU_spread) + SIMU_fee

                            executedOrders.Add(o)

                            Dim t As DegiroTransaction = New DegiroTransaction With {
                                .ticker = o.ticker,
                                .action = "Achat",
                                .quantity = o.quantity,
                                .dat = Date.UtcNow,
                                .fee = SIMU_fee,
                                .isin = assetInfo(o.ticker).ISIN,
                                .pru = o.stopPrice * (1 + SIMU_spread)
                            }

                            dbg.info(" New transaction " & StructToString(t))


                            transactions.Add(t)

                            positions.Add(New DegiroPosition With {
                                .ticker = o.ticker,
                                .pru = o.stopPrice,
                                .quantity = o.quantity,
                                .currentTotalValue = o.limit * o.quantity
                                })

                        End If
                End Select

            Next

            For Each order As DegiroOrder In executedOrders
                orders.Remove(order)
            Next

            accountWinLooseMoula = accountTotalMoula - 10000

            ' produceTradesStructuresFromEverything()

        End Sub

        Public Sub SIMU_placeOrUpdateOrder(ticker As String, qty As Integer, orderAction As String, limit As Double, stopPrice As Double)

            Dim order As DegiroOrder = Nothing

            For Each o As DegiroOrder In orders
                If o.ticker = ticker And o.quantity = qty And o.orderAction = orderAction Then
                    order = o
                    order.limit = limit
                    order.stopPrice = stopPrice
                    Exit For
                End If
            Next
            If order.quantity = 0 Then
                order = New DegiroOrder With {
                    .ticker = ticker,
                    .dat = Date.UtcNow,
                    .isin = assetInfo(ticker).ISIN,
                    .limit = limit,
                    .stopPrice = stopPrice,
                    .orderAction = orderAction,
                    .quantity = qty
                }
                orders.Add(order)
            End If
            '  RandPause(1600, 3200)
        End Sub

        Public Sub SIMU_cancelOrder(ticker As String, qty As Integer, orderAction As String)
            For Each o As DegiroOrder In orders
                If o.ticker = ticker And o.quantity = qty And o.orderAction = orderAction Then
                    orders.Remove(o)
                    Exit For
                End If
            Next
            RandPause(1600, 3200)
        End Sub

        ' --------------------------------------------------------------------------------------------------
        ' --------------------------------------------------------------------------------------------------
        ' REAL
        Public Sub loadPastData()
            loadPastFromFiles()

            ' process some merge if any
            produceTradesStructuresFromEverything()
        End Sub

        Public Sub loadPastFromFiles()
            ' load all transactions without being in a completed trade
            previousTransactions = transactionsFromFiles()
            transactions = cloneTransactionList(previousTransactions)
            ' load full-completed trades
            previousTrades = tradesFromFiles()
            trades = cloneTradeList(previousTrades)

            reloadPastFromFileRequest = False
        End Sub


        ' updateAll Read
        Public Sub updateAll()
            Dim start As Date = Date.UtcNow

            If Not Edge.switchTab(Edge.TabEnum.DEGIRO_POSITONS) Then
                status = StatusEnum.OFFLINE
                Exit Sub
            End If
            If status = StatusEnum.OFFLINE Then status = StatusEnum.ONLINE

            Dim body As String

            ' expect position tab opened
            body = KMOut.selectAllCopy()
            updateAccountDataFromBody(body)
            updatePositions(body)

            Edge.switchTab(Edge.TabEnum.DEGIRO_ORDERS)
            body = KMOut.selectAllCopy()
            updateOrders(body)

            Edge.switchTab(Edge.TabEnum.DEGIRO_TRANSACTIONS)
            body = KMOut.selectAllCopy()
            updateTransactions(body)

            ' create trade aggreagation
            produceTradesStructuresFromEverything()

            lastUpdate = Date.UtcNow()

            dbg.info("Updated degiro data within " & Math.Round(Date.UtcNow.Subtract(start).TotalMilliseconds) & "ms")

        End Sub


        Public Sub updateAccountDataFromBody(body As String)
            Dim nextIs As String = ""

            For Each l As String In body.Split(vbCrLf)

                'dbg.info(l)
                'dbg.info(nextIs)

                'If l.Contains("€ ") Then

                '    dbg.info(l.Replace("€ ", "").Trim())
                '    dbg.info(parseMoney(l))
                'End If

                Select Case nextIs
                    Case "accountTotalMoula"
                        accountTotalMoula = parseMoney(l)
                    Case "accountPositionsMoula"
                        accountPositionsMoula = parseMoney(l)
                    Case "accountCashMoula"
                        accountCashMoula = parseMoney(l)
                    Case "accountWinLooseMoula"
                        accountWinLooseMoula = parseMoney(l)
                        Exit For
                End Select

                nextIs = ""
                If l.Contains("Total Compte") Then nextIs = "accountTotalMoula"
                If l.Contains("Portefeuille") Then nextIs = "accountPositionsMoula"
                If l.Contains("EUR") Then nextIs = "accountCashMoula"
                If l.Contains("L Cumulée") Then nextIs = "accountWinLooseMoula"
            Next


            dbg.info(vbCrLf & vbCrLf &
                         "| accountTotalMoula      = " & accountTotalMoula.ToString("   0.00") & " €" & vbCrLf &
                         "| accountPositionsMoula  = " & accountPositionsMoula.ToString("   0.00") & " €" & vbCrLf &
                         "| accountCashMoula       = " & accountCashMoula.ToString("   0.00") & " €" & vbCrLf &
                         "| accountWinLooseMoula   = " & accountWinLooseMoula.ToString("   0.00") & " €" & vbCrLf)

            ' FrmMain.degiroLabel.Text = "cash: " & accountCashMoula & "€ positions: " & accountPositionsMoula & "€"

            'Recherche par nom, ISIN ou ticker

            'Aide & assistance
            'Dépôt / Retrait
            'Ordre
            'Total Compte
            '€ 150,53
            'Portefeuille
            '€ 145,23

            'EUR
            '€ 5,30


            'Espace libre
            '€ 5,30


            'P/ L Cumulée
            '€ +114,46



        End Sub

        Public Sub updatePositions(body As String)

            positions.Clear()

            For Each l As String In body.Split(vbCrLf)
                If Not l.Contains(" | ") Then Continue For

                Dim split As List(Of String) = splitTableLine(l)

                ' printListOfString(split)
                If split.Count <> 14 Then Continue For

                If split.ElementAt(1) <> "|" Then Continue For

                Dim position As DegiroPosition = New DegiroPosition With {
                                  .ticker = split.ElementAt(0),
                                  .isin = split.ElementAt(2),
                                  .quantity = Integer.Parse(split.ElementAt(3)),
                                  .currentTotalValue = parseMoney(split.ElementAt(6)),
                                  .pru = parseMoney(split.ElementAt(7))
                                  }

                dbg.info(" ->: " & StructToString(position))

                positions.Add(position)

            Next
            ' 3OIL | IE00BMTM6B32	5	€ 28,91	EUR	144,55	28,50	0,00	0,00%	+2,05 (+1,43%)	-0,95	15	

        End Sub

        'position


        'Recherche par nom, ISIN ou ticker

        'Aide & assistance
        'Dépôt / Retrait
        'Ordre
        'Total Compte
        '€ 150,53
        'Portefeuille
        '€ 145,23

        'EUR
        '€ 5,30


        'Espace libre
        '€ 5,30


        'P/L Cumulée
        '€ +114,46




        'Portefeuille

        'Trackers (ETF)
        'Produit

        'Ticker | ISIN
        'Qté
        'Cours
        'Devise
        'Valeur

        'PRU

        'P/L €
        'P/L %

        'P/L latent
        ' €

        'Total P/L
        '€

        'A
        'V

        'Wisdomtree Wti Crude Oil 3X Daily Lev
        'D
        '3OIL | IE00BMTM6B32	5	€ 28,91	EUR	144,55	28,50	-0,95	-6,31%	+2,05 (+1,43%)	-0,95	15	

        '1

        'Compensation espèces

        'Description	Devise	Total
        'Compensation courue	EUR	0,68
        'Compensation en attente	EUR	0,00
        'Total compensation versée	EUR	0,00
        '© 2024 - flatexDEGIRO Bank Dutch Branch
        '- Données fournies par : LSEG Data & Analytics
        ' | Données prix de : Infront Financial Technology; Euronext Chi-X Bats


        ' ==========================================================================================================
        ' ==========================================================================================================
        ' ==========================================================================================================

        Public Sub updateOrders(body As String)

            orders.Clear()

            For Each l As String In body.Split(vbCrLf)
                If Not l.Contains(" | ") Then Continue For

                Dim split As List(Of String) = splitTableLine(l)

                'printListOfString(split)
                If split.Count <> 13 Then Continue For
                If split.ElementAt(3) <> "|" Then Continue For

                Dim daySplit As String() = split.ElementAt(0).Split("/")
                ' 30/08/2024 16:49:27 3OIL | IE00BMTM6B32 MIL Vente 5 33,00 — 165,00 5 0
                Dim order As DegiroOrder = New DegiroOrder With {
                               .ticker = split.ElementAt(2),
                               .isin = split.ElementAt(4),
                               .dat = Date.Parse(daySplit.ElementAt(1) & "/" & daySplit.ElementAt(0) & "/" & daySplit.ElementAt(2) & " " & split.ElementAt(1)),
                               .orderAction = split.ElementAt(6),
                               .quantity = split.ElementAt(7),
                               .limit = parseMoney(split.ElementAt(8)),
                               .stopPrice = parseMoney(split.ElementAt(9))
                              }

                orders.Add(order)

                dbg.info(" ->: " & StructToString(order))
            Next

            'date ticker isin placeBoursiere action qté limitPrix€ 33,00 stop(€ —) valeur ouvert execution
            '30/08/2024 16:49:27	3OIL | IE00BMTM6B32	MIL	Vente	5	€ 33,00	€ —	165,00	5	0	
        End Sub

        'orders:

        '        Recherche par nom, ISIN ou ticker

        'Aide & assistance
        'Dépôt / Retrait
        'Ordre
        'Total Compte
        '€ 150,53
        'Portefeuille
        '€ 145,23

        'EUR
        '€ 5,30


        'Espace libre
        '€ 5,30


        'P/L Cumulée
        '€ +114,46

        'Ordres en cours
        '1
        'Dernières transactions du jour
        '1



        'Messages17
        'Ordres
        'Transactions
        'Compte
        'Dépréciation du portefeuille
        'Documents
        'Ordres
        'Rechercher un produit
        'Produit
        'Date
        'Ticker | ISIN
        'Place boursière
        'Action
        'Qté
        'Limite
        'Prix stop
        'Valeur
        'Ouvert
        'Exécuté



        'Wisdomtree Wti Crude Oil 3X Daily Lev
        'D
        '30/08/2024 16:49:27	3OIL | IE00BMTM6B32	MIL	Vente	5	€ 33,00	€ —	165,00	5	0	

        '1

        '© 2024 - flatexDEGIRO Bank Dutch Branch
        '- Données fournies par : LSEG Data & Analytics
        ' | Données prix de : Infront Financial Technology; Euronext Chi-X Bats
        'Commentaire



        ' placeOrder(name, amount, type as Enum of limit/stoploss/stopbuy) as Order  where name is in a know value from enum, then we will find isin, and degiro id number

        ' cancelOrder(order as Order) as boolean 

        ' ==========================================================================================================
        ' ==========================================================================================================
        ' ==========================================================================================================

        ' transactions

        Public Sub updateTransactions(body As String)
            transactions.Clear()

            ' do we need to clone ?
            transactions = previousTransactions

            dbg.info("transaction from file count=" & transactions.Count)

            Dim lineStep As Integer = 0

            Dim transaction As New DegiroTransaction

            For Each l As String In body.Split(vbCrLf)
                Dim split As List(Of String) = splitTableLine(l)

                ' dbg.info("STEP" & lineStep)
                ' printListOfString(split)

                ' mono line
                If split.Count = 15 AndAlso split.ElementAt(3) = "|" Then
                    Dim daySplit As String() = split.ElementAt(0).Split("/")

                    ' 18/08/2020 16:10:07 CSCO | US17275R1023 NDQ Achat 1 42,00 -42,00 -35,17 1,1943 -0,04 -0,50 -35,70
                    transaction = New DegiroTransaction With {
                        .ticker = split.ElementAt(2),
                        .dat = Date.Parse(daySplit.ElementAt(1) & "/" & daySplit.ElementAt(0) & "/" & daySplit.ElementAt(2) & " " & split.ElementAt(1)),
                        .isin = split.ElementAt(4),
                        .action = split.ElementAt(6),
                        .quantity = Math.Abs(Integer.Parse(split.ElementAt(7))),
                        .quantityFragmentSold = 0,
                        .pru = Math.Abs(parseMoney(split.ElementAt(10)) / Integer.Parse(split.ElementAt(7))),
                        .fee = Math.Abs(parseMoney(split.ElementAt(13)))
                    }

                    'save to file if not found
                    If Not File.Exists(transactionToFilePath(transaction)) And Not File.Exists(completedTransactionToFilePath(transaction)) Then
                        File.WriteAllText(transactionToFilePath(transaction), serializeTransaction(transaction))
                    End If

                    transactions.Add(transaction)
                    dbg.info(" ->: " & StructToString(transaction))

                End If


                ' multi line
                If split.Count = 6 AndAlso lineStep = 0 AndAlso split.ElementAt(3) = "|" Then
                    lineStep = 1

                    ' 27/07/2023 14:39:06
                    Dim daySplit As String() = split.ElementAt(0).Split("/")

                    transaction = New DegiroTransaction With {
                        .ticker = split.ElementAt(2),
                        .dat = Date.Parse(daySplit.ElementAt(1) & "/" & daySplit.ElementAt(0) & "/" & daySplit.ElementAt(2) & " " & split.ElementAt(1)),
                        .isin = split.ElementAt(4),
                        .action = "",
                        .quantity = 0,
                        .pru = 2,
                        .quantityFragmentSold = 0,
                        .fee = 3
                    }

                    Continue For
                End If

                If lineStep = 1 Then
                    lineStep = 2
                    Continue For
                End If
                ' Achat	5	 28,50	 -142,50	 -142,50	—	—	 -3,00	 -145,50
                If lineStep = 2 Then
                    'For Each ss As String In split
                    '    dbg.info(ss)
                    'Next
                    ' dbg.info(Math.Abs(parseMoney(split.ElementAt(4)) / Integer.Parse(split.ElementAt(1))))
                    transaction.action = split.ElementAt(0)
                    transaction.quantity = Math.Abs(Integer.Parse(split.ElementAt(1)))
                    transaction.pru = Math.Abs(parseMoney(split.ElementAt(4)) / Integer.Parse(split.ElementAt(1)))
                    transaction.fee = Math.Abs(parseMoney(split.ElementAt(7)))
                    lineStep = 0

                    'save to file if not found
                    If Not File.Exists(transactionToFilePath(transaction)) And Not File.Exists(completedTransactionToFilePath(transaction)) Then
                        File.WriteAllText(transactionToFilePath(transaction), serializeTransaction(transaction))
                    End If

                    transactions.Add(transaction)
                    dbg.info(" ->: " & StructToString(transaction))
                    Continue For
                End If
                lineStep = 0
            Next
        End Sub


        'Wisdomtree Wti Crude Oil 3X Daily Lev
        'D
        '30/08/2024 15:07:36	3OIL | IE00BMTM6B32	MIL	
        'ETFP
        'Achat	5	€ 28,50	€ -142,50	€ -142,50	—	—	€ -3,00	€ -145,50	
        'A
        'V

        ' 18/08/2020 16:10:07	CSCO | US17275R1023	NDQ		Achat	1	$ 42,00	$ -42,00	€ -35,17	1,1943	€ -0,04	€ -0,50	€ -35,70	


        'WisdomTree Natural Gas - EUR Daily Hedged
        'D
        '09/08/2023 14:03:21	ENGS | JE00B6XF0923	MIL	
        'ETFP
        'Vente	-392	€ 0,65	€ 254,80	€ 254,80	—	—	€ -3,00	€ 251,80	
        'A
        'V

        'Wisdomtree Wti Crude Oil 3X Daily Short
        'D
        '31/07/2023 11:14:04	3OIS | IE00BMTM6C49	MIL	
        'ETFP
        'Vente	-1044	€ 0,54	€ 563,76	€ 563,76	—	—	€ -3,00	€ 560,76	
        'A
        'V

        'Wisdomtree Wti Crude Oil 3X Daily Short
        'D
        '27/07/2023 16:04:55	3OIS | IE00BMTM6C49	MIL	
        'ETFP
        'Achat	1044	€ 0,56	€ -584,64	€ -584,64	—	—	€ -3,00	€ -587,64	
        'A
        'V

        'WisdomTree WTI Crude Oil ETC
        'A
        '27/07/2023 14:39:06	CRUD | GB00B15KXV33	MIL	
        'ETFP
        'Vente	-68	€ 8,50	€ 578,00	€ 578,00	—	—	€ -3,00	€ 575,00	
        'A
        'V

        'WisdomTree Natural Gas - EUR Daily Hedged
        'D
        '01/06/2023 15:01:58	ENGS | JE00B6XF0923	MIL	
        'ETFP
        'Achat	392	€ 0,51	€ -199,92	€ -199,92	—	—	€ -3,00	€ -202,92	
        'A
        'V

        'WisdomTree WTI Crude Oil ETC
        'A
        '03/05/2023 14:55:26	CRUD | GB00B15KXV33	MIL	
        'ETFP
        'Achat	68	€ 7,30	€ -496,40	€ -496,40	—	—	€ -1,00	€ -497,40	
        'A
        'V

        'WisdomTree WTI Crude Oil ETC
        'A
        '04/04/2023 14:02:13	CRUD | GB00B15KXV33	MIL	
        'ETFP
        'Vente	-7	€ 8,60	€ 60,20	€ 60,20	—	—	€ -1,00	€ 59,20	
        'A
        'V

        'Cisco Systems
        'A
        '20/03/2023 14:30:02	CSCO | US17275R1023	NDQ	
        'XNAS
        'Vente	-1	$ 50,10	$ 50,10	€ 46,72	1,0723	€ -0,12	€ -1,00	€ 45,61	
        'A
        'V

        'WisdomTree WTI Crude Oil ETC
        'A
        '20/03/2023 09:04:02	CRUD | GB00B15KXV33	MIL	
        'ETFP
        'Achat	7	€ 7,099	€ -49,69	€ -49,69	—	—	€ -1,00	€ -50,69	

        '1

        'Résultats par page
        '© 2024 - flatexDEGIRO Bank Dutch Branch
        '- Données fournies par : LSEG Data & Analytics
        ' | Données prix de : Infront Financial Technology; Euronext Chi-X Bats



        Dim reloadPastFromFileRequest As Boolean = 0

        ' and save files with it, update transaction files name too
        Public Sub produceTradesStructuresFromEverything()
            Dim hasBeenBuild As Boolean = False
            Dim quantityLeft As Integer = 0
            reloadPastFromFileRequest = False

            'create trades from past transactions, orders

            trades.Clear()
            trades = previousTrades

            '  dbg.info("trade from file count=" & trades.Count)

            ' ----------------------------------------------------------------------------------------------------------
            ' PROCESS COMPLETED

            For Each transactionVente As DegiroTransaction In transactions
                If transactionVente.action <> "Vente" Then Continue For

                dbg.info("Trade merge: found Vente " & StructToString(transactionVente) & " Now I need to find Achats with quantity " & transactionVente.quantity)

                ' find Achat that fit
                ' first filter
                Dim transactionAchats As New List(Of DegiroTransaction)
                For Each transactionAchat As DegiroTransaction In transactions
                    If transactionAchat.action <> "Achat" Or transactionAchat.ticker <> transactionVente.ticker Then Continue For
                    transactionAchats.Add(transactionAchat)
                Next

                ' among them pick one with exact same quantity baught and sold

                hasBeenBuild = False
                For Each transactionAchat As DegiroTransaction In transactionAchats
                    If transactionAchat.quantity = transactionVente.quantity Then
                        dbg.info(" -> Found Exact Achat ! " & StructToString(transactionAchat))
                        newTradeFromTransactions(transactionAchat, transactionVente)
                        hasBeenBuild = True
                        Exit For
                    End If
                Next
                ' go to next vente
                If hasBeenBuild Then Continue For

                ' among them pick one with exact fragment
                hasBeenBuild = False
                For Each transactionAchat As DegiroTransaction In transactionAchats
                    If transactionAchat.quantity = transactionVente.quantity - transactionVente.quantityFragmentSold Then
                        dbg.info(" -> Found Fragment Achat ! " & StructToString(transactionAchat))
                        newTradeFromTransactions(transactionAchat, transactionVente)
                        hasBeenBuild = True
                        Exit For
                    End If
                Next
                ' go to next vente
                If hasBeenBuild Then Continue For

                'else fragment with achats



                '   newTradeFromTransactions(transactionAchats, transactionVente)
            Next

            ' reload
            If reloadPastFromFileRequest Then loadPastFromFiles()

            'we should now have 0 "Vente" left
            Dim venteLeft As New List(Of DegiroTransaction)
            For Each transactionVente As DegiroTransaction In transactions
                If transactionVente.action <> "Vente" Then Continue For
                venteLeft.Add(transactionVente)
            Next
            If venteLeft.Count > 0 Then
                dbg.fail(venteLeft.Count & " ventes left after trade creation :")
                For Each t As DegiroTransaction In venteLeft
                    dbg.info("  >>" & StructToString(t))
                Next
            End If


            ' ----------------------------------------------------------------------------------------------------------
            ' PROCESS BUY DONE - NO SELL YET - WITH ORDER EXIST


            ' all transactions belongs to transaction with no trade completed
            ' from transaction, resolve to new trades and complete them
            ' if some, drop transaction from transactions

            ' from ventes transaction, find achat transaction associated, create completed trades from it, delete transaction use in that
            ' update file name of transaction in completed trade as attachedToTrade


            ' from sell orders, find achat transaction associated, create incompleted trade from it,  delete transaction used in that, should have no transaction left (or not!)
            ' we may have achat transaction without sell orders

            ' from buy order, create incompleted trade from it



            ' post check : from trade with buy done but sell not done, we should find exactly the same thing as the portefeuille, alerte else


        End Sub


        Public Sub newTradeFromTransactions(transactionAchat As DegiroTransaction, transactionVente As DegiroTransaction)
            'just in case
            If transactionAchat.ticker.Contains("Fake") Then Exit Sub

            Dim trade As New DegiroTrade With {
                .ticker = transactionAchat.ticker,
                .isin = transactionAchat.isin,
                .buyDone = True,
                .buyFee = transactionAchat.fee,
                .buyDate = transactionAchat.dat,
                .quantity = transactionAchat.quantity,
                .pru = transactionAchat.pru,
                .sellPricePerUnit = transactionVente.pru,
                .sellDone = True,
                .sellFee = transactionVente.fee,
                .sellDate = transactionVente.dat
            }

            trade.perfPerc = trade.sellPricePerUnit / trade.pru
            trade.totalPlusValue = 1.0 * trade.quantity * trade.sellPricePerUnit / trade.pru - trade.buyFee - trade.sellFee


            'save to file if not found
            If Not File.Exists(tradeToFilePath(trade)) And status <> StatusEnum.SIMU Then
                File.WriteAllText(tradeToFilePath(trade), serializeTrade(trade))
                File.Move(transactionToFilePath(transactionAchat), completedTransactionToFilePath(transactionAchat))
                File.Move(transactionToFilePath(transactionVente), completedTransactionToFilePath(transactionVente))
                ' + notify SLACK
            End If
            trades.Add(trade)
            dbg.info(" new trade completed : " & StructToString(trade))

            reloadPastFromFileRequest = True
        End Sub


        ' ----------------------------------------------------------------------------------------------------------
        ' UPDATE FrmMain Trade panel

        Public Sub updateTradePanelUI()
            ' FrmMain.DataGridViewTrades.Rows.Clear()

            Dim activeTradeString As String = ""

            ' dbg.info(orders.Count)
            For Each order In orders
                If order.orderAction <> "Achat" Then Continue For
                If order.limit > 0 Then
                    activeTradeString &= "LIMIT_BUY " & order.ticker & " q" & order.quantity & " limit" & order.limit & " cur" & TradingView.getPrice(assetInfo(order.ticker)).price & " " & 0 & "% away" & vbCrLf
                Else
                    activeTradeString &= "STOP_BUY " & order.ticker & " q" & order.quantity & " stop" & order.stopPrice & " cur" & TradingView.getPrice(assetInfo(order.ticker)).price & " " & 0 & "% away" & vbCrLf
                End If
            Next

            activeTradeString &= vbCrLf

            For Each order In orders
                If order.orderAction <> "Vente" Then Continue For

                If order.limit > 0 Then
                    activeTradeString &= "LIMIT_SELL " & order.ticker & " q" & order.quantity & " limit" & order.limit & " cur" & TradingView.getPrice(assetInfo(order.ticker)).price & " " & 0 & "% away" & vbCrLf
                Else
                    activeTradeString &= "STOP_SELL " & order.ticker & " q" & order.quantity & " stop" & order.stopPrice & " cur" & TradingView.getPrice(assetInfo(order.ticker)).price & " " & 0 & "% away" & vbCrLf
                End If
            Next


            For Each position In positions
                activeTradeString &= "POSITION " & position.ticker & " q" & position.quantity & " pru" & position.pru & vbCrLf
            Next


            activeTradeString &= vbCrLf

            For Each trade In trades
                If Date.UtcNow.Subtract(trade.sellDate).TotalHours > 24 Then Continue For

                activeTradeString &= "COMPLETED " & trade.ticker & " q" & trade.quantity & " pru" & trade.pru & " sold" & trade.sellPricePerUnit.ToString(" 0.0") & " perf" & (100 * (trade.perfPerc - 1)).ToString("0.00") & "%" & vbCrLf
            Next

            If FrmMain.LblActiveTrades.Text <> activeTradeString Then FrmMain.LblActiveTrades.Text = activeTradeString

            'TRY_BUY_LIMIT 3OIL q5 limit13.31 cur14 3.2% away
            'TRY_BUY_STOP 3OIS q5 stop13.31 cur13 0.2% away
            'STOP_SELL 4FFA q21 pru18.1 stop18.9 cur19.3 1.2% away
            'LIMIT_SELL 4FFA q21 pru18.1 limit21 cur19.3 4.6% away
            'POSITION 2FA q5 pru12 cur8.5
            'COMPLETED 3OIL q12 pru51.2 -5.1% -543.32€ 2h ago
            Application.DoEvents()
        End Sub


        Public Sub createFakeData()
            If CST.COMPILED Then
                MsgBox("attempt to create fake data in compile env")
                Exit Sub
            End If

            If status <> StatusEnum.OFFLINE Then
                MsgBox("attempt to create fake data while global status is not offline")
                Exit Sub
            End If

            dbg.info("CREATE FAKE ORDERS/POSITONS FOR UI")

            ' lonly buy orders
            orders.Add(New DegiroOrder With {
                       .ticker = "Fake3OIL",
                       .dat = Date.UtcNow.AddHours(-0.2),
                       .limit = "25",
                       .orderAction = "Achat",
                       .quantity = 8
                       })

            orders.Add(New DegiroOrder With {
                       .ticker = "Fake3USL",
                       .dat = Date.UtcNow.AddHours(-0.05),
                       .limit = "85",
                       .orderAction = "Achat",
                       .quantity = 231
                       })

            ' position with stop loss
            positions.Add(New DegiroPosition With {
                         .ticker = "Fake3OIS",
                         .pru = 31.5,
                         .quantity = 28,
                         .currentTotalValue = 28 * 29.5
                         })

            orders.Add(New DegiroOrder With {
                       .ticker = "Fake3OIS",
                       .dat = Date.UtcNow.AddHours(-0.54),
                       .stopPrice = 29,
                       .orderAction = "Vente"
                       })


            ' position with limit sell
            positions.Add(New DegiroPosition With {
                        .ticker = "Fake3OISi",
                        .pru = 25,
                        .quantity = 32,
                        .currentTotalValue = 28 * 32.5
                        })

            orders.Add(New DegiroOrder With {
                       .ticker = "Fake3OISi",
                       .dat = Date.UtcNow.AddHours(-321.54),
                       .limit = 412,
                       .orderAction = "Vente"
                       })

            ' completed trades

            trades.Add(New DegiroTrade With {
              .buyDate = Date.UtcNow.AddHours(-1321.54),
              .buyDone = True,
              .sellDone = True,
              .sellDate = Date.UtcNow.AddHours(-0.4),
              .buyFee = 3,
              .sellFee = 3,
              .pru = 12,
              .sellPricePerUnit = 15,
              .perfPerc = 1.15,
              .quantity = 54,
              .ticker = "FakeSIS",
              .totalPlusValue = 412
            })


        End Sub

        ' ==========================================================================================================
        ' ==========================================================================================================
        ' ==========================================================================================================


        ' this is home page
        ' <Marché - Personal - Microsoft​ Edge>  process msedge

        ' https://trader.degiro.nl/trader/#/portfolio/assets
        ' <Portefeuille - Personal - Microsoft​ Edge>  process msedge

        ' https://trader.degiro.nl/trader/#/orders/open
        ' <Ordres en cours and 1 more page - Personal - Microsoft​ Edge>  process msedge


        ' https://trader.degiro.nl/trader/#/transactions?fromDate=2024-08-01&toDate=2024-08-31&groupTransactionsByOrder=false&activePeriodType=LastMonthIncludingToday
        ' <Transaction - Personal - Microsoft​ Edge>


        ' https://trader.degiro.nl/trader/#/favourites
        ' <Favoris and 1 more page - Personal - Microsoft​ Edge>  process msedge (7060)

        ' https://trader.degiro.nl/trader/#/products/18744180/overview
        ' <Wisdomtree Wti Crude Oil 3X Daily Lev – € 29,83 | -1,28 (-4,11%) and 1 more page - Personal - Microsoft​ Edge>  process msedge
        ' ca update le win title tout seul aussi ? :)

        'Private namesMatching As New List(Of String) From {"Marché", "Portefeuille", "Ordres en cours"}

        'Enum DegiroStateEnum
        '    UNKNOWN
        '    NO_EDGE
        '    EDGE_PAIRED
        '    LOGIN
        'End Enum

        'Public degiroState As DegiroStateEnum = DegiroStateEnum.UNKNOWN
        'Public handler As IntPtr

        'Public marcheProcess As New Process
        'Dim portefeuilleProcess As New Process
        'Dim ordersProcess As New Process

        'Public Sub setupWindows()

        '    ' Marché
        '    Dim marcheProcess As Process = Edge.findEdgeContainingName("Marché")

        '    If marcheProcess Is Nothing Then
        '        dbg.fail("Can't find an edge degiro Marché")
        '        Exit Sub
        '    End If

        '    User32.bringToFront(marcheProcess.MainWindowHandle)
        '    User32.setPos(marcheProcess.MainWindowHandle, 0, 0, 1200, 600)

        '    Exit Sub

        '    Dim portefeuilleProcess As Process = Edge.findEdgeContainingName("Portefeuille")
        '    If portefeuilleProcess Is Nothing Then
        '        Edge.createTab("https://trader.degiro.nl/trader/#/portfolio/assets")
        '        portefeuilleProcess = Edge.findEdgeContainingName("Portefeuille")
        '    End If

        '    User32.bringToFront(portefeuilleProcess.MainWindowHandle)
        '    User32.setPos(portefeuilleProcess.MainWindowHandle, 900, 0, 400, 600)

        '    Dim ordersProcess As Process = Edge.findEdgeContainingName("Ordres en cours")
        '    If ordersProcess Is Nothing Then
        '        Edge.createTab("https://trader.degiro.nl/trader/#/orders/open")
        '        ordersProcess = Edge.findEdgeContainingName("Portefeuille")
        '    End If

        '    User32.bringToFront(ordersProcess.MainWindowHandle)
        '    User32.setPos(ordersProcess.MainWindowHandle, 900 + 400, 0, 400, 600)



        'End Sub

        'Public Sub pairToEdgeDegiro()
        '    Dim p As Process = Edge.findEdgeContainingOneNames(namesMatching)

        '    ' first time found
        '    If Not p Is Nothing And degiroState = DegiroStateEnum.UNKNOWN Then

        '        dbg.info(p.MainWindowTitle)

        '        Dim handler As IntPtr = p.MainWindowHandle

        '        'windowHandle = User32.FindWindow(Nothing, windowTitleName)
        '        'windowPaired = windowHandle <> Nothing And windowHandle.ToInt32 > 0

        '        ' handler = User32.FindWindow(Nothing, p.MainWindowTitle)
        '        ' handler = User32.FindWindow(Nothing, "Marché")



        '        dbg.info("Degiro window handler " & p.Id & " " & handler.ToString & " rect=" & User32.getWindowPos(handler).ToString)
        '        User32.bringToFront(handler)
        '        User32.setPos(handler, 10, 10, 500, 800)

        '    End If


        '    'If p Is Nothing Then
        '    '    degiroState = DegiroStateEnum.NO_EDGE
        '    'Else

        '    '    degiroState = DegiroStateEnum.EDGE_PAIRED
        '    'End If



        'End Sub



        'Public Function checkLoggedIn() As Boolean
        '    Dim ok As Boolean = Edge.switchTab(Edge.TabEnum.DEGIRO_POSITONS)

        '    If ok Then
        '        FrmMain.LblDegiroState.Text = "DEGIRO LOGGED_IN"
        '        ' also set pos
        '    Else
        '        FrmMain.LblDegiroState.Text = "DEGIRO DISCONNECTED"
        '        FrmMain.LblDegiroState.BackColor = Color.LightCyan
        '    End If

        '    ' update ui from ok

        '    Return ok
        'End Function

    End Module
End Namespace