Imports System.Reflection.Metadata
Imports System.Runtime.InteropServices

Namespace Degiro



    Module Degiro
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

        Private namesMatching As New List(Of String) From {"Marché", "Portefeuille", "Ordres en cours", "Favoris"}

        Enum DegiroStateEnum
            UNKNOWN
            NO_EDGE
            EDGE_PAIRED
            LOGIN
        End Enum

        Public degiroState As DegiroStateEnum = DegiroStateEnum.UNKNOWN

        Public handler As IntPtr

        Public Const SWP_NOMOVE As Short = &H2
        Public Const SWP_NOSIZE As Short = 1
        Public Const SWP_NOZORDER As Short = &H4
        Public Const SWP_SHOWWINDOW As Short = &H40
        ReadOnly HWND_BOTTOM As IntPtr = New IntPtr(1)

        <DllImport("user32.dll", EntryPoint:="SetWindowPos")>
        Public Function SetWindowPos(
 hWnd As IntPtr,
 hWndInsertAfter As IntPtr,
 x As Int32, y As Int32, cx As Int32, cy As Int32, wFlags As Int32) As IntPtr
        End Function


        Public Sub pairToEdgeDegiro()
            ' Dim p As Process =
            '   System.Diagnostics.Process.Start("C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", "https://trader.degiro.nl/login/fr#/login")

            Pause(1000)
            Dim Processes As Process() = Process.GetProcessesByName("msedge")

            For Each p As Process In Processes

                If Not p.MainWindowTitle.Contains("Personal - Microsoft​ Edge") Then Continue For

                dbgMain.info("edge available: <" & p.MainWindowTitle & "> process " & p.ProcessName & " (" & p.Id & ") " & User32.getWindowPos(p.MainWindowHandle).ToString)

            Next


            'Dim handle As IntPtr = p.MainWindowHandle
            'dbgMain.info(handle.ToString)

            'SetWindowPos(handle, HWND_BOTTOM, 200, 200, 0, 0, SWP_NOZORDER Or SWP_NOSIZE Or SWP_SHOWWINDOW)

            Exit Sub

            'https://learn.microsoft.com/en-us/answers/questions/378971/vb-net-and-the-windows-10-microsoft-edge-browser
            'Dim procs As Process() = Process.GetProcessesByName("Microsoft Edge")

            'For Each p As Process In procs
            '    If p.MainWindowHandle = IntPtr.Zero Then Continue For
            '    Dim element As AutomationElement = AutomationElement.FromHandle(p.MainWindowHandle)
            '    If element Is Nothing Then Continue For
            '    Dim conditions As Condition = New AndCondition(New PropertyCondition(AutomationElement.ProcessIdProperty, p.Id), New PropertyCondition(AutomationElement.IsControlElementProperty, True), New PropertyCondition(AutomationElement.IsContentElementProperty, True), New PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit))
            '    Dim elementx As AutomationElement = element.FindFirst(TreeScope.Descendants, conditions)
            '    Console.WriteLine(TryCast((CType(elementx.GetCurrentPattern(ValuePattern.Pattern), ValuePattern)).Current.Value, String))
            'Next


            'Dim procs As Process() = Process.GetProcessesByName("msedge")

            'For Each pp As Process In procs
            '    If pp.MainWindowHandle = IntPtr.Zero Then Continue For
            '    dbgMain.info("KKKK available: <" & pp.MainWindowTitle & ">  process " & pp.ProcessName & " (" & pp.Id & ")")
            '    '    Dim element As AutomationElement = AutomationElement.FromHandle(p.MainWindowHandle)
            '    '    If element Is Nothing Then Continue For
            '    '    Dim conditions As Condition = New AndCondition(New PropertyCondition(AutomationElement.ProcessIdProperty, p.Id), New PropertyCondition(AutomationElement.IsControlElementProperty, True), New PropertyCondition(AutomationElement.IsContentElementProperty, True), New PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit))
            '    '    Dim elementx As AutomationElement = element.FindFirst(TreeScope.Descendants, conditions)
            '    '    Console.WriteLine(TryCast((CType(elementx.GetCurrentPattern(ValuePattern.Pattern), ValuePattern)).Current.Value, String))
            'Next

            'Dim p As Process = WinHandle.findEdgeContainingOneNames(namesMatching)



            ' first time found
            'If Not p Is Nothing And degiroState = DegiroStateEnum.UNKNOWN Then

            '    dbgMain.info(p.MainWindowTitle)

            '    Dim handler As IntPtr = 14908 ' p.MainWindowHandle

            '    'windowHandle = User32.FindWindow(Nothing, windowTitleName)
            '    'windowPaired = windowHandle <> Nothing And windowHandle.ToInt32 > 0

            '    '     handler = User32.FindWindow(Nothing, p.MainWindowTitle)
            '    '  handler = User32.FindWindow(Nothing, "Marché")



            '    dbgMain.info("Degiro window handler " & p.Id & " " & handler.ToString & " rect=" & User32.getWindowPos(handler).ToString)
            '    User32.bringToFront(handler)

            'End If


            'If p Is Nothing Then
            '    degiroState = DegiroStateEnum.NO_EDGE
            'Else

            '    degiroState = DegiroStateEnum.EDGE_PAIRED
            'End If



        End Sub



    End Module
End Namespace