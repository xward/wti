Imports System.Reflection.Metadata
Imports System.Runtime
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.JavaScript.JSType
Imports System.IO

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

        Public lastUpdate As Date


        Public Function cloneTransactionList(transactions As List(Of DegiroTransaction))
            Dim ret As New List(Of DegiroTransaction)
            For Each t In transactions
                ret.Add(t)
            Next
            Return ret
        End Function

        Public Function cloneTradeList(transactions As List(Of DegiroTrade))
            Dim ret As New List(Of DegiroTrade)
            For Each t In transactions
                ret.Add(t)
            Next
            Return ret
        End Function

        Public Sub loadPastFromFiles()
            ' load all transactions without being in a completed trade
            previousTransactions = transactionsFromFiles()
            transactions = cloneTransactionList(previousTransactions)
            ' load full-completed trades
            previousTrades = tradesFromFiles()
            trades = cloneTradeList(previousTrades)
        End Sub

        Public Sub loadPastData()
            loadPastFromFiles()

            ' process some merge if any
            produceTradesStructuresFromEverything()
        End Sub

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

            FrmMain.degiroLabel.Text = "cash: " & accountCashMoula & "€ positions: " & accountPositionsMoula & "€"

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

                Dim split As List(Of String) = splitLine(l)

                ' printListOfString(split)
                If split.Count <> 14 Then Continue For

                If split.ElementAt(1) <> "|" Then Continue For

                Dim position As DegiroPosition = New DegiroPosition With {
                                  .ticker = split.ElementAt(0),
                                  .isin = split.ElementAt(2),
                                  .quantity = Integer.Parse(split.ElementAt(3)),
                                  .totalValue = parseMoney(split.ElementAt(6)),
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

                Dim split As List(Of String) = splitLine(l)

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





        ' updateOrders as boolean
        ' updatePositions as boolean

        ' placeOrder(name, amount, type as Enum of limit/stoploss/stopbuy) as Order  where name is in a know value from enum, then we will find isin, and degiro id number

        ' cancelOrder(order as Order) as boolean 

        ' ==========================================================================================================
        ' ==========================================================================================================
        ' ==========================================================================================================

        ' transactions

        Public Function splitLine(l As String) As List(Of String)
            l = l.Replace("$", "").Replace("€", "").Trim
            Dim tmpSplit As String() = l.Split({" ", "	"}, StringSplitOptions.None)
            Dim split As New List(Of String)
            For Each s As String In tmpSplit
                If s.Trim.Length > 0 Then split.Add(s)
            Next
            Return split
        End Function

        Public Sub printListOfString(l As List(Of String))
            dbg.info("elem count= " & l.Count & " " & String.Join(" ", l))
        End Sub


        Public Sub updateTransactions(body As String)
            transactions.Clear()

            ' do we need to clone ?
            transactions = previousTransactions

            dbg.info("transaction from file count=" & transactions.Count)

            Dim lineStep As Integer = 0

            Dim transaction As DegiroTransaction

            For Each l As String In body.Split(vbCrLf)
                Dim split As List(Of String) = splitLine(l)

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
                        .quantity = Integer.Parse(split.ElementAt(7)),
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
                    transaction.quantity = Integer.Parse(split.ElementAt(1))
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


        ' and save files with it, update transaction files name too
        Public Sub produceTradesStructuresFromEverything()
            Dim hasBeenBuild As Boolean = False
            Dim quantityLeft As Integer = 0

            'create trades from past transactions, orders

            trades.Clear()
            trades = previousTrades

            dbg.info("trade from file count=" & trades.Count)

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
                        dbg.info(" -> Fount Exact Achat ! " & StructToString(transactionAchat))
                        ' buildTrade(vente, achat)
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
                        dbg.info(" -> Fount Fragment Achat ! " & StructToString(transactionAchat))
                        ' buildTrade(vente, achat)
                        hasBeenBuild = True
                        Exit For
                    End If
                Next
                ' go to next vente
                If hasBeenBuild Then Continue For

                'else fragment with achats



                ' buildTrade(vente, achats)
            Next

            ' reload
            loadPastFromFiles()


            'we should now have 0 "Vente" left
            Dim venteLeft As New List(Of DegiroTransaction)
            For Each transactionAchat As DegiroTransaction In transactions
                If transactionAchat.action <> "Achat" Then Continue For
                venteLeft.Add(transactionAchat)
            Next
            If venteLeft.Count > 0 Then
                dbg.fail(venteLeft.Count & " ventes left after trade creation :")
                For Each t As DegiroTransaction In venteLeft
                    dbg.info("  >>" & StructToString(t))
                Next
            End If


            ' ----------------------------------------------------------------------------------------------------------
            ' PROCESS BUY DONE


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