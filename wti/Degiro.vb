Imports System.Reflection.Metadata
Imports System.Runtime.InteropServices

Namespace Degiro


    Public Structure Order
        Dim id As String
        Dim asset As AssetEnum
        Dim orderType As OrderTypeEnum
        ' all the values

        Dim referToPosition As String
    End Structure

    Public Structure Position
        Dim id As String
        Dim asset As AssetEnum
        ' can be negative
        Dim gain As Double

        ' previous gain
    End Structure

    Public Enum AssetEnum
        WTI3x
        WTI3xShort
    End Enum

    Public Enum OrderTypeEnum
        LIMIT_SELL
        LIMIT_BUY
        ' stop loss
        STOP_SELL
        STOP_BUY
    End Enum


    Module Degiro
        Public orders As New List(Of Order)
        Public positions As New List(Of Position)


        Public Function checkLoggedIn() As Boolean
            Dim ok As Boolean = Edge.switchTab(Edge.TabEnum.DEGIRO_POSITONS)

            If ok Then
                FrmMain.LblDegiroState.Text = "DEGIRO LOGGED_IN"
            Else
                FrmMain.LblDegiroState.Text = "DEGIRO DISCONNECTED"
                FrmMain.LblDegiroState.BackColor = Color.LightCyan
            End If

            ' update ui from ok

            Return ok
        End Function

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

        Private namesMatching As New List(Of String) From {"Marché", "Portefeuille", "Ordres en cours"}

        Enum DegiroStateEnum
            UNKNOWN
            NO_EDGE
            EDGE_PAIRED
            LOGIN
        End Enum

        Public degiroState As DegiroStateEnum = DegiroStateEnum.UNKNOWN
        Public handler As IntPtr

        Public marcheProcess As New Process
        Dim portefeuilleProcess As New Process
        Dim ordersProcess As New Process

        Public Sub setupWindows()

            ' Marché
            Dim marcheProcess As Process = Edge.findEdgeContainingName("Marché")

            If marcheProcess Is Nothing Then
                dbg.fail("Can't find an edge degiro Marché")
                Exit Sub
            End If

            User32.bringToFront(marcheProcess.MainWindowHandle)
            User32.setPos(marcheProcess.MainWindowHandle, 0, 0, 1200, 600)

            Exit Sub

            Dim portefeuilleProcess As Process = Edge.findEdgeContainingName("Portefeuille")
            If portefeuilleProcess Is Nothing Then
                Edge.createTab("https://trader.degiro.nl/trader/#/portfolio/assets")
                portefeuilleProcess = Edge.findEdgeContainingName("Portefeuille")
            End If

            User32.bringToFront(portefeuilleProcess.MainWindowHandle)
            User32.setPos(portefeuilleProcess.MainWindowHandle, 900, 0, 400, 600)

            Dim ordersProcess As Process = Edge.findEdgeContainingName("Ordres en cours")
            If ordersProcess Is Nothing Then
                Edge.createTab("https://trader.degiro.nl/trader/#/orders/open")
                ordersProcess = Edge.findEdgeContainingName("Portefeuille")
            End If

            User32.bringToFront(ordersProcess.MainWindowHandle)
            User32.setPos(ordersProcess.MainWindowHandle, 900 + 400, 0, 400, 600)



        End Sub

        Public Sub pairToEdgeDegiro()
            Dim p As Process = Edge.findEdgeContainingOneNames(namesMatching)

            ' first time found
            If Not p Is Nothing And degiroState = DegiroStateEnum.UNKNOWN Then

                dbg.info(p.MainWindowTitle)

                Dim handler As IntPtr = p.MainWindowHandle

                'windowHandle = User32.FindWindow(Nothing, windowTitleName)
                'windowPaired = windowHandle <> Nothing And windowHandle.ToInt32 > 0

                ' handler = User32.FindWindow(Nothing, p.MainWindowTitle)
                ' handler = User32.FindWindow(Nothing, "Marché")



                dbg.info("Degiro window handler " & p.Id & " " & handler.ToString & " rect=" & User32.getWindowPos(handler).ToString)
                User32.bringToFront(handler)
                User32.setPos(handler, 10, 10, 500, 800)

            End If


            'If p Is Nothing Then
            '    degiroState = DegiroStateEnum.NO_EDGE
            'Else

            '    degiroState = DegiroStateEnum.EDGE_PAIRED
            'End If



        End Sub



    End Module
End Namespace