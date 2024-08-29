Imports System.Runtime.InteropServices

''' <summary>
''' Helper class containing User32 API functions
''' </summary>
Public Class User32
    <StructLayout(LayoutKind.Sequential)> _
    Public Structure WIN_RECT_PTR
        Public left As Integer
        Public top As Integer
        Public right As Integer
        Public bottom As Integer
    End Structure
    <DllImport("user32.dll", SetLastError:=True)> _
    Public Shared Function FindWindow(lpClassName As String, lpWindowName As String) As IntPtr
    End Function
    <DllImport("user32.dll")> _
    Public Shared Function GetWindowDC(ByVal hWnd As IntPtr) As IntPtr
    End Function
    <DllImport("user32.dll")> _
    Public Shared Function ReleaseDC(ByVal hWnd As IntPtr, ByVal hDC As IntPtr) As IntPtr
    End Function
    <DllImport("user32.dll")> _
    Public Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef rect As WIN_RECT_PTR) As IntPtr
    End Function
    Public Shared Function getWindowPos(ByVal hwnd As Integer) As Rectangle ' easier to call
        Dim lolRect As User32.WIN_RECT_PTR
        GetWindowRect(hwnd, lolRect)
        Return New Rectangle(lolRect.left, lolRect.top, lolRect.right - lolRect.left, lolRect.bottom - lolRect.top)
    End Function

    ' good old getkey
    <DllImport("user32.dll")>
    Public Shared Function GetAsyncKeyState(ByVal vKey As System.Windows.Forms.Keys) As Short
    End Function

    'fonction donnant le handle de la fenetre du bureau
    Public Declare Function GetDesktopWindow Lib "User32" () As Integer

    'fonction donnant le handle de la fenêtre active
    Public Declare Function GetForegroundWindow Lib "User32" () As Integer

    'met une fenetre au premier plan
    Private Declare Function SetForegroundWindow Lib "user32" (ByVal hwnd As Long) As Integer

    Private Declare Function SetFocus Lib "user32" (ByVal hwnd As Long) As Integer

    'locker / delocker fenetre
    Public Declare Function EnableWindow Lib "User32" ( _
                     ByVal hWnd As Long, _
                     ByVal fEnable As Long) As Integer

    Public Const SWP_NOSIZE = &H1
    Public Const SWP_NOMOVE = &H2
    Public Const SWP_NOZORDER = &H4
    Public Const SWP_NOREDRAW = &H8
    Public Const SWP_NOACTIVATE = &H10
    Public Const SWP_DRAWFRAME = &H20
    Public Const SWP_FRAMECHANGED = &H20
    Public Const SWP_SHOWWINDOW = &H40
    Public Const SWP_HIDEWINDOW = &H80
    Public Const SWP_NOCOPYBITS = &H100
    Public Const SWP_NOOWNERZORDER = &H200
    Public Const SWP_NOREPOSITION = &H400
    Public Const SWP_NOSENDCHANGING = &H400
    Public Const SWP_DEFERERASE = &H2000
    Public Const SWP_ASYNCWINDOWPOS = &H4000
    <DllImport("user32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)> _
    Public Shared Function SetWindowPos( _
              ByVal hWnd As IntPtr, _
              ByVal hWndInsertAfter As IntPtr, _
              ByVal X As Int32, _
              ByVal Y As Int32, _
              ByVal cx As Int32, _
              ByVal cy As Int32, _
              ByVal uFlags As Int32) _
              As Boolean
    End Function
    Public Shared Sub setPos(ByVal hwnd As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
        SetWindowPos(New IntPtr(hwnd), New IntPtr(-1), x, y, w, h, SWP_NOZORDER)
    End Sub


    Public Shared Sub bringToFront(ByVal hwnd As Integer)
        Dim hdle As Integer = GetForegroundWindow()
        If hdle = hwnd Then Exit Sub
        SetForegroundWindow(hwnd)
        SetFocus(hwnd)
    End Sub

    Enum ShowWindowCommands As Integer
        ''' <summary>
        ''' Hides the window and activates another window.
        ''' </summary>
        Hide = 0
        ''' <summary>
        ''' Activates and displays a window. If the window is minimized or
        ''' maximized, the system restores it to its original size and position.
        ''' An application should specify this flag when displaying the window
        ''' for the first time.
        ''' </summary>
        Normal = 1
        ''' <summary>
        ''' Activates the window and displays it as a minimized window.
        ''' </summary>
        ShowMinimized = 2
        ''' <summary>
        ''' Maximizes the specified window.
        ''' </summary>
        Maximize = 3
        ' is this the right value?
        ''' <summary>
        ''' Activates the window and displays it as a maximized window.
        ''' </summary>      
        ShowMaximized = 3
        ''' <summary>
        ''' Displays a window in its most recent size and position. This value
        ''' is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except
        ''' the window is not actived.
        ''' </summary>
        ShowNoActivate = 4
        ''' <summary>
        ''' Activates the window and displays it in its current size and position.
        ''' </summary>
        Show = 5
        ''' <summary>
        ''' Minimizes the specified window and activates the next top-level
        ''' window in the Z order.
        ''' </summary>
        Minimize = 6
        ''' <summary>
        ''' Displays the window as a minimized window. This value is similar to
        ''' <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the
        ''' window is not activated.
        ''' </summary>
        ShowMinNoActive = 7
        ''' <summary>
        ''' Displays the window in its current size and position. This value is
        ''' similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the
        ''' window is not activated.
        ''' </summary>
        ShowNA = 8
        ''' <summary>
        ''' Activates and displays the window. If the window is minimized or
        ''' maximized, the system restores it to its original size and position.
        ''' An application should specify this flag when restoring a minimized window.
        ''' </summary>
        Restore = 9
        ''' <summary>
        ''' Sets the show state based on the SW_* value specified in the
        ''' STARTUPINFO structure passed to the CreateProcess function by the
        ''' program that started the application.
        ''' </summary>
        ShowDefault = 10
        ''' <summary>
        '''  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
        ''' that owns the window is not responding. This flag should only be
        ''' used when minimizing windows from a different thread.
        ''' </summary>
        ForceMinimize = 11
    End Enum
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Public Shared Function ShowWindow(ByVal hwnd As IntPtr, ByVal nCmdShow As ShowWindowCommands) As Boolean
    End Function

End Class
