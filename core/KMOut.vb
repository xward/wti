Namespace KMOut
    Module KMOut
        Public Function selectAllCopy() As String

            Clipboard.Clear()

            InputManager.Keyboard.KeyDown(Keys.ControlKey)
            Pause(ctrlPause)
            InputManager.Keyboard.KeyPress(Keys.A)
            Pause(ctrlPause)
            InputManager.Keyboard.KeyPress(Keys.C)
            Pause(ctrlPause)
            InputManager.Keyboard.KeyUp(Keys.ControlKey)
            Pause(ctrlPause)

            ' else clipboard get text will fail
            Application.DoEvents()
            Pause(ctrlPause)

            Return Clipboard.GetText()
        End Function

        Public Sub pasteText(text As String)
            Clipboard.SetText(text)
            Application.DoEvents()
            Pause(ctrlPause)
            ctrl(Keys.V)
        End Sub

        Public Sub copy()
            ctrl(Keys.C)
        End Sub

        Dim ctrlPause As Integer = 35
        Public Sub ctrl(k As Keys)
            InputManager.Keyboard.KeyDown(Keys.ControlKey)
            Pause(ctrlPause)
            InputManager.Keyboard.KeyPress(k)
            Pause(ctrlPause)
            InputManager.Keyboard.KeyUp(Keys.ControlKey)
            Pause(ctrlPause)
            Application.DoEvents()
            Pause(ctrlPause)
        End Sub

        Public Sub enter()
            InputManager.Keyboard.KeyPress(Keys.Enter)
        End Sub

    End Module
End Namespace

