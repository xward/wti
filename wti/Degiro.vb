Imports System.Reflection.Metadata
Imports System.Runtime.InteropServices

Namespace Degiro

    Module Degiro
        Public tradableMoula As Double
        Public orders As New List(Of Order)
        Public positions As New List(Of Position)


        Public Function checkLoggedIn() As Boolean
            Dim ok As Boolean = Edge.switchTab(Edge.TabEnum.DEGIRO_POSITONS)

            If ok Then
                FrmMain.LblDegiroState.Text = "DEGIRO LOGGED_IN"
                ' also set pos
            Else
                FrmMain.LblDegiroState.Text = "DEGIRO DISCONNECTED"
                FrmMain.LblDegiroState.BackColor = Color.LightCyan
            End If

            ' update ui from ok

            Return ok
        End Function

        Public Sub bumpStayLoggedIn()

        End Sub



        Public Function updatePositions() As Boolean
            Edge.switchTab(Edge.TabEnum.DEGIRO_POSITONS)


            Dim body As String = KMOut.selectAllCopy()

            dbg.info(body)


        End Function

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

        Public Function updateOrders() As Boolean
            Edge.switchTab(Edge.TabEnum.DEGIRO_ORDERS)



            Dim body As String = KMOut.selectAllCopy()

            dbg.info(body)






        End Function

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



    End Module
End Namespace