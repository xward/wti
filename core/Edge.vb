﻿
Imports Microsoft.VisualBasic.Devices


Namespace Edge
    Public Enum OpenModeEnum
        AS_TAB
        AS_WINDOW
    End Enum

    ' https://trader.degiro.nl/trader/#/portfolio/assets
    ' <Portefeuille - Personal - Microsoft​ Edge>  process msedge

    ' https://trader.degiro.nl/trader/#/orders/open
    ' <Ordres en cours and 1 more page - Personal - Microsoft​ Edge>  process msedge

    ' https://trader.degiro.nl/trader/#/transactions
    ' <Transaction en cours and 1 more page - Personal - Microsoft​ Edge>  process msedge

    Public Enum TabEnum
        ' current stuff I own
        DEGIRO_POSITONS
        ' active orders
        DEGIRO_ORDERS
        ' transactions history
        DEGIRO_TRANSACTIONS
    End Enum


    Module Edge
        ' tab = window
        ' only one process of edge for all edge tabs or windows

        Const EDGE_PATH As String = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
        Public edgeProcess As Process
        Const PROCESS_POST_PAUSE As Integer = 2500
        Public edgeWindowRect As New Rectangle(0, 0, 1200, CST.COMPILED And CST.SCREEN.Height Or 800)

        Public Sub ensureRunning()
            updateEdgeProcess()

            If edgeProcess Is Nothing Then
                edgeProcess = System.Diagnostics.Process.Start(EDGE_PATH)
                Pause(PROCESS_POST_PAUSE)
            End If
            setWinPosition()
        End Sub

        Public Sub setWinPosition()
            Dim rect As Rectangle = User32.getWindowPos(edgeProcess.MainWindowHandle)
            If rect.ToString <> edgeWindowRect.ToString Then
                bringToFront()
                User32.setPos(edgeProcess.MainWindowHandle, edgeWindowRect.X, edgeWindowRect.Y, edgeWindowRect.Width, edgeWindowRect.Height)
                Pause(300)
            End If
        End Sub


        Public Sub createTab(url As String, Optional openMode As OpenModeEnum = OpenModeEnum.AS_TAB)
            Dim extra As String = ""
            If openMode = OpenModeEnum.AS_WINDOW Then extra = " --new-window "

            System.Diagnostics.Process.Start(EDGE_PATH, extra & url)
            Pause(PROCESS_POST_PAUSE)
            updateEdgeProcess()

            bringToFront()
        End Sub

        ' create tab if not exist
        Public Sub createTabIfNotExist(name As String, url As String, openMode As OpenModeEnum, Optional pos As Rectangle = Nothing)
            If switchTab(name) Then Exit Sub
            dbg.info("Creating new EDGE tab " & name)
            createTab(url, openMode)

            '  If Not IsNothing(pos) Then User32.setPos(edgeProcess.MainWindowHandle, pos.X, pos.Y, pos.Width, pos.Height)
        End Sub

        ' switch to tab or window via ctrl+maj+a where window title containing name
        ' return true on success

        Public Function switchTab(tab As TabEnum) As Boolean
            Select Case tab
                Case TabEnum.DEGIRO_POSITONS
                    If switchTab("portefeuille") Then
                        'init position if needed
                        setWinPosition()
                        Return True
                    Else
                        Return False
                    End If
                Case TabEnum.DEGIRO_ORDERS
                    Edge.createTabIfNotExist("Ordres en cours", "https://trader.degiro.nl/trader/#/orders/open", OpenModeEnum.AS_TAB)
                    Return True

                Case TabEnum.DEGIRO_TRANSACTIONS
                    Edge.createTabIfNotExist("Transaction", "https://trader.degiro.nl/trader/#/transactions", OpenModeEnum.AS_TAB)
                    Return True
            End Select
            Return False
        End Function


        Public Function switchTab(name As String) As Boolean
            bringToFront()
            Pause(200)

            Dim keyPause As Integer = 100

            InputManager.Keyboard.KeyDown(Keys.ControlKey)
            Pause(keyPause)
            InputManager.Keyboard.KeyDown(Keys.LShiftKey)
            Pause(keyPause)
            InputManager.Keyboard.KeyPress(Keys.A)
            Pause(keyPause)
            InputManager.Keyboard.KeyUp(Keys.LShiftKey)
            Pause(keyPause)
            InputManager.Keyboard.KeyUp(Keys.ControlKey)
            Pause(200)


            ' paste
            KMOut.pasteText(name)
            Pause(100)

            KMOut.enter()
            Pause(100)

            updateEdgeProcess()

            'unselect any or close search if not found
            InputManager.Keyboard.KeyPress(Keys.Escape)
            Pause(100)

            'printAllEdge()
            'dbg.info(edgeProcess.MainWindowTitle)
            Return edgeProcess.MainWindowTitle.ToUpper.Contains(name.ToUpper)
        End Function

        Public Sub bringToFront()
            User32.bringToFront(edgeProcess.MainWindowHandle)
        End Sub

        Public Function currentEdgeWindowTitleInclude(name As String) As Boolean
            Return edgeProcess.MainWindowTitle.ToUpper.Contains(name.ToUpper)

        End Function

        ' ------------------------------------------------------------------------------------------------------------------

        Public Function extractBetween(src As String, aBlock As String, bBlock As String) As String
            Return src.Split(aBlock).ElementAt(1).Split(bBlock).ElementAt(0)
        End Function


        ' ------------------------------------------------------------------------------------------------------------------
        Public Function updateEdgeProcess() As Process
            ' re-fetch process, userfull to update process window's title
            For Each p As Process In Process.GetProcessesByName("msedge")
                If Not p.MainWindowTitle.Contains("Personal - Microsoft​ Edge") Then Continue For
                edgeProcess = p
                Return p
            Next
            Return Nothing
        End Function


        'Public Function findEdgeContainingName(name As String) As Process
        '    Return findEdgeContainingOneNames(New List(Of String)({name}))
        'End Function

        'Public Function findEdgeContainingOneNames(names As List(Of String)) As Process
        '    For Each p As Process In Process.GetProcessesByName("msedge")
        '        If Not p.MainWindowTitle.Contains("Personal - Microsoft​ Edge") Then Continue For
        '        For Each name As String In names
        '            If p.MainWindowTitle.Contains(name) Then Return p
        '        Next
        '    Next
        '    Return Nothing
        'End Function

        Public Sub printAllEdge()
            Dim Processes As Process() = Process.GetProcessesByName("msedge")

            For Each p As Process In Processes

                If Not p.MainWindowTitle.Contains("Personal - Microsoft​ Edge") Then Continue For
                dbg.info("EDGE available: <" & p.MainWindowTitle & "> process " & p.ProcessName & " (" & p.Id & ") " & User32.getWindowPos(p.MainWindowHandle).ToString)

                'User32.bringToFront(p.MainWindowHandle)
                'User32.setPos(p.MainWindowHandle, 10, 10, 500, 800)
            Next
        End Sub
    End Module
End Namespace
