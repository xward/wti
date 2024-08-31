Imports System.Reflection.Metadata
Imports System.Runtime.InteropServices

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

        Public lastUpdate As Date

        Public Sub updateAll()
            Dim start As Date = Date.UtcNow

            If Not Edge.switchTab(Edge.TabEnum.DEGIRO_POSITONS) Then
                status = StatusEnum.OFFLINE
                Exit Sub
            End If
            If status = StatusEnum.OFFLINE Then status = StatusEnum.ONLINE

            ' expect position tab opened
            Dim body As String = KMOut.selectAllCopy()
            ' dbg.info(body)

            updateAccountDataFromBody(body)
            updatePositions(body)

            Edge.switchTab(Edge.TabEnum.DEGIRO_ORDERS)
            body = KMOut.selectAllCopy()
            updateOrders(body)


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


            dbg.info(vbCrLf &
                         "accountTotalMoula       = " & accountTotalMoula & " €" & vbCrLf &
                          "accountPositionsMoula  = " & accountPositionsMoula & " €" & vbCrLf &
                           "accountCashMoula      = " & accountCashMoula & " €" & vbCrLf &
                            "accountWinLooseMoula = " & accountWinLooseMoula & " €" & vbCrLf)

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
                If Not l.Contains(" | ") Or Not l.Contains("€") Then Continue For

                Dim split As String() = l.Split({" ", "	"}, StringSplitOptions.None)
                If split.Length <> 16 Then Continue For
                'For Each s As String In split
                '    dbg.info(">" & s & "<")
                'Next
                If split.ElementAt(1) <> "|" Then Continue For
                If split.ElementAt(4) <> "€" Then Continue For
                If split.ElementAt(6) <> "EUR" Then Continue For

                dbg.info("position: " & l)

                Dim position As DegiroPosition = New DegiroPosition With {
                                  .ticker = split.ElementAt(0),
                                  .isin = split.ElementAt(2),
                                  .quantity = Integer.Parse(split.ElementAt(3)),
                                  .totalValue = parseMoney(split.ElementAt(7)),
                                  .pru = parseMoney(split.ElementAt(8))
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
                If Not l.Contains(" | ") Or Not l.Contains("€") Then Continue For

                Dim split As String() = l.Split({" ", "	"}, StringSplitOptions.None)
                'dbg.info(split.Length)
                If split.Length <> 16 Then Continue For
                'For Each s As String In split
                '    dbg.info(">" & s & "<")
                'Next
                If split.ElementAt(3) <> "|" Then Continue For

                dbg.info("order: " & l)

                Dim daySplit As String() = split.ElementAt(0).Split("/")
                Dim order As DegiroOrder = New DegiroOrder With {
                               .ticker = split.ElementAt(2),
                               .isin = split.ElementAt(4),
                               .dat = Date.Parse(daySplit.ElementAt(1) & "/" & daySplit.ElementAt(0) & "/" & daySplit.ElementAt(1) & " " & split.ElementAt(1)),
                               .orderAction = split.ElementAt(6),
                               .quantity = split.ElementAt(7),
                               .limit = parseMoney(split.ElementAt(9)),
                               .stopPrice = parseMoney(split.ElementAt(11))
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