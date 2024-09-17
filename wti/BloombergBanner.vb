
Imports System.Runtime.InteropServices.JavaScript.JSType

Namespace BloombergBanner
    Module BloombergBanner
        Private img As New Bitmap(1, 1)
        Private g As Graphics
        Private rendering As Boolean = False
        Private renderCount As Integer = 0
        Private asyncRenderTimer As Timer

        Public elapsed As Double

        Private defaultFont As New Font("Cascadia Mono", 14)
        Private gridPen As Pen = New Pen(Color.FromArgb(100, Color.Gray))

        Private textToPrint As String = "COURAGE STAY FOCUS BUY si sp500 10%"
        Private xScroll As Integer = 0

        Public Sub start()

            asyncRenderTimer = New Timer
            asyncRenderTimer.Interval = 50
            AddHandler asyncRenderTimer.Tick, AddressOf AsyncRenderTimer_Tick
            asyncRenderTimer.Enabled = True
        End Sub

        Private Sub AsyncRenderTimer_Tick(sender As Object, e As EventArgs)
            asyncRenderTimer.Enabled = False
            If Not rendering Then render()
            If status <> StatusEnum.INTERRUPT Then asyncRenderTimer.Enabled = True
        End Sub

        Public Sub render()
            rendering = True
            renderCount += 1
            Dim start As Date = Date.UtcNow()
            checkResized()
            ' init
            paintItBlack()

            ' 11.14
            Dim textW As Integer = 12 * textToPrint.Length

            xScroll -= 1
            If xScroll > textW Then xScroll = 0

            For x = -textW + xScroll To img.Width Step textW
                writeText(New Point(x, 10), " " & textToPrint & "    ", Color.Black, Color.Transparent)
            Next

            FrmMain.PictureBoxBloombergBanner.Image = img
            elapsed = Date.UtcNow.Subtract(start).TotalMilliseconds
            Application.DoEvents()

            ' temps
            Dim fps As Double = 1000 / Math.Max(1, elapsed)
            rendering = False
        End Sub

        Private Sub checkResized()
            If FrmMain.PictureBoxBloombergBanner.Size.ToString() <> img.Size.ToString() Then
                img = New Bitmap(FrmMain.PictureBoxBloombergBanner.Width, FrmMain.PictureBoxBloombergBanner.Height)
                g = Graphics.FromImage(img)
            End If
        End Sub


        Private Sub paintItBlack()
            g.FillRectangle(New SolidBrush(Color.LightGray), New Rectangle(0, 0, img.Width, img.Height))
        End Sub

        Public Sub writeText(ByVal pt As Point, ByVal txt As String, ByVal col As Color, ByVal backgroundColor As Color, Optional fontSize As Single = 14)
            'If backgroundColor <> Color.Transparent Then
            '    For y = pt.Y To pt.Y + 12
            '        drawLine(New Point(pt.X, y), New Point(pt.X + 9 * txt.Length, y), backgroundColor)
            '    Next
            'End If
            Dim brch As New SolidBrush(col)
            Dim font As Font = New Font(defaultFont.FontFamily, fontSize)
            g.DrawString(txt, font, brch, New Point(pt.X - 2, pt.Y - 4))
        End Sub

    End Module



End Namespace
