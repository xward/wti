
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

    Public Enum TabEnum
        DEGIRO_POSITONS
        DEGIRO_ORDERS
    End Enum

    Module Edge
        ' tab = window
        ' only one process of edge for all edge tabs or windows

        Const EDGE_PATH As String = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
        Public edgeProcess As Process
        Const PROCESS_POST_PAUSE As Integer = 1000

        Public Sub ensureRunning()
            updateEdgeProcess()
            If edgeProcess Is Nothing Then
                edgeProcess = System.Diagnostics.Process.Start(EDGE_PATH)
                Pause(PROCESS_POST_PAUSE)
            End If
        End Sub

        Public Sub createTab(url As String, Optional openMode As OpenModeEnum = OpenModeEnum.AS_TAB)
            Dim extra As String = ""
            If openMode = OpenModeEnum.AS_WINDOW Then extra = " --new-window "

            System.Diagnostics.Process.Start(EDGE_PATH, extra & url)
            Pause(PROCESS_POST_PAUSE)
            updateEdgeProcess()
        End Sub

        ' create tab if not exist
        Public Sub createTabIfNotExist(name As String, url As String, openMode As OpenModeEnum, pos As Rectangle)
            If switchTab(name) Then Exit Sub
            createTab(url, openMode)

            bringToFront()
            User32.setPos(edgeProcess.MainWindowHandle, pos.X, pos.Y, pos.Width, pos.Height)
        End Sub

        ' switch to tab or window via ctrl+maj+a where window title containing name
        ' return true on success

        Public Function switchTab(tab As TabEnum) As Boolean
            Select Case tab
                Case TabEnum.DEGIRO_ORDERS
                    Return switchTab("ordre")
                Case TabEnum.DEGIRO_POSITONS
                    Return switchTab("portefeuille")
            End Select
            Return False
        End Function

        Public Function switchTabPlaceOrder(tab As Degiro.AssetEnum) As Boolean

            Return False
        End Function

        Public Function switchTab(name As String) As Boolean
            bringToFront()
            Pause(100)

            Clipboard.SetText(name)

            InputManager.Keyboard.KeyDown(Keys.ControlKey)
            Pause(15)
            InputManager.Keyboard.KeyDown(Keys.LShiftKey)
            Pause(15)
            InputManager.Keyboard.KeyPress(Keys.A)
            Pause(15)
            InputManager.Keyboard.KeyUp(Keys.LShiftKey)
            Pause(15)
            InputManager.Keyboard.KeyUp(Keys.ControlKey)
            Pause(15)

            ' paste
            InputManager.Keyboard.KeyDown(Keys.ControlKey)
            Pause(15)
            InputManager.Keyboard.KeyPress(Keys.V)
            Pause(15)
            InputManager.Keyboard.KeyUp(Keys.ControlKey)
            Pause(15)
            ' enter
            InputManager.Keyboard.KeyPress(Keys.Enter)

            Pause(100)
            updateEdgeProcess()
            'printAllEdge()
            'dbg.info(edgeProcess.MainWindowTitle)
            Return edgeProcess.MainWindowTitle.ToUpper.Contains(name.ToUpper)
        End Function

        Public Sub bringToFront()
            User32.bringToFront(edgeProcess.MainWindowHandle)
        End Sub

        ' ------------------------------------------------------------------------------------------------------------------
        Public Function updateEdgeProcess() As Process
            For Each p As Process In Process.GetProcessesByName("msedge")
                If Not p.MainWindowTitle.Contains("Personal - Microsoft​ Edge") Then Continue For
                edgeProcess = p
                Return p
            Next
            Return Nothing
        End Function

        Public Function findEdgeContainingName(name As String) As Process
            Return findEdgeContainingOneNames(New List(Of String)({name}))
        End Function

        Public Function findEdgeContainingOneNames(names As List(Of String)) As Process
            For Each p As Process In Process.GetProcessesByName("msedge")
                If Not p.MainWindowTitle.Contains("Personal - Microsoft​ Edge") Then Continue For
                For Each name As String In names
                    If p.MainWindowTitle.Contains(name) Then Return p
                Next
            Next
            Return Nothing
        End Function

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
