'Namespace GraphDrawzz


'    Module GraphDraw
'        Public imgTop As New Bitmap(1, 1)
'        Public imgBottom As New Bitmap(1, 1)
'        Public gTop As Graphics
'        Public gBottom As Graphics

'        Public currentAsset As AssetInfos
'        Public fps As Double
'        Public elapsed As Double

'        Private gridPen As Pen = New Pen(Color.FromArgb(100, Color.Gray))


'        ' later: merge histos, useful for 3 month
'        ' later: bougies

'        Public Sub render()
'            Dim start As Date = Date.UtcNow
'            resizePicture()
'            ' init
'            paintItWhite(imgTop, gTop)
'            paintItWhite(imgBottom, gBottom)

'            ' actual stuff
'            renderAsset()

'            'finalize
'            FrmMain.PictureBoxTopGraph.Image = imgTop
'            FrmMain.PictureBoxBottomGraph.Image = imgBottom

'            Application.DoEvents()

'            elapsed = Date.UtcNow.Subtract(start).TotalMilliseconds
'            fps = 1000 / Math.Max(1, elapsed)
'            FrmMain.ToolStripStatusLabelDrawFps.Text = Math.Round(elapsed) & "ms (" & Math.Round(fps) & " fps)"
'        End Sub


'        ' ------------------------------------------------------------------------------------------------------------------------
'        Public Sub renderAsset()
'            ' implem AssetHistory = list of assetPrice, that load everything we know on this asset ( only what I recorded, for now ), max 4 month history for now (graph doesn't need more). Use this in simulation instead, implem min/max auto, index 5j auto 3mo auto
'            ' implem filter per date min max in assetHistory

'            ' find min/max of it percentage
'            ' define échelle


'            Dim hPerc As Double = 8


'            ' group point by date_wide / bottomPicture.width - x
'            '

'            drawHorizontalGrid(imgTop, gTop, imgTop.Height / 2, 60)
'            drawVerticalGrid(imgTop, gTop, imgTop.Width / 2, imgTop.Width / (3 * 4))


'            drawHorizontalGrid(imgBottom, gBottom, imgBottom.Height / 2, 60)
'            drawVerticalGrid(imgBottom, gBottom, imgBottom.Width / 2, imgBottom.Width / (5 * 24 / 12))



'            'imgTop.SetPixel(10, 10, Color.Red)
'        End Sub

'        ' ------------------------------------------------------------------------------------------------------------------------
'        ' draw routines

'        Private Sub drawHorizontalGrid(ByRef image As Bitmap, ByRef g As Graphics, zeroY As Integer, stepp As Integer)
'            For y = zeroY To imgBottom.Height Step stepp
'                g.DrawLine(gridPen, New Point(0, y), New Point(image.Width, y))
'            Next
'            For y = zeroY - stepp To 0 Step -stepp
'                g.DrawLine(gridPen, New Point(0, y), New Point(image.Width, y))
'            Next
'        End Sub

'        Private Sub drawVerticalGrid(ByRef image As Bitmap, ByRef g As Graphics, zeroX As Integer, stepp As Integer)
'            For x = zeroX To imgBottom.Width Step stepp
'                g.DrawLine(gridPen, New Point(x, 0), New Point(x, image.Height))
'            Next
'            For x = zeroX - stepp To 0 Step -stepp
'                g.DrawLine(gridPen, New Point(x, 0), New Point(x, image.Height))
'            Next
'        End Sub


'        ' ------------------------------------------------------------------------------------------------------------------------

'        Private Sub resizePicture()
'            If FrmMain.PanelGraphTop.Size.ToString <> imgTop.Size.ToString Then
'                imgTop = New Bitmap(FrmMain.PanelGraphTop.Size.Width, FrmMain.PanelGraphTop.Size.Height)
'                gTop = Graphics.FromImage(imgTop)
'            End If
'            If FrmMain.PanelGraphBottom.Size.ToString <> imgBottom.Size.ToString Then
'                imgBottom = New Bitmap(FrmMain.PanelGraphBottom.Size.Width, FrmMain.PanelGraphBottom.Size.Height)
'                gBottom = Graphics.FromImage(imgBottom)
'            End If
'        End Sub

'        Public Sub paintItWhite(ByRef image As Bitmap, ByRef g As Graphics)
'            g.FillRectangle(New SolidBrush(Color.FromArgb(255, 253, 241, 230)), New Rectangle(0, 0, image.Width, image.Height))
'        End Sub

'        '  Color.BlanchedAlmond 
'        ' Color.FromArgb(255, 250, 237, 220)
'    End Module

'End Namespace

''Dim b As New SolidBrush(Color.Black)


''Using br = New LinearGradientBrush(New Rectangle(50, 50, 10, 10), Color.Red, Color.Orange, 25)
''e.Graphics.FillRectangle(br, New Rectangle(50, 50, 10, 10))
''End Using

'' drawLine(New Point(0, pt1.Y), New Point(curWImg.Width - 1, pt1.Y), color, width)

''Public Sub drawImage(ByRef img As Bitmap, ByVal location As Point)
''    g.DrawImage(img, location)
''End Sub

''Public Sub writeTextOnImg(ByVal pt As Point, ByVal txt As String, ByVal col As Color, ByVal backgroundColor As Color, Optional fontSize As Single = 14)
''    If backgroundColor <> Color.Transparent Then
''        For y = pt.Y To pt.Y + 12
''            drawLine(New Point(pt.X, y), New Point(pt.X + 9 * txt.Length, y), backgroundColor)
''        Next
''    End If
''    Dim brch As New SolidBrush(col)
''    fontCalibri = New Font(fontCalibri.FontFamily, fontSize)
''    g.DrawString(txt, fontCalibri, brch, New Point(pt.X - 2, pt.Y - 4))
''End Sub

''Public Sub drawLine(ByVal pt1 As Point, ByVal pt2 As Point, ByVal color As Color, Optional ByVal width As Integer = 1)
''    If pt1.X = pt2.X And pt1.Y = pt2.Y Then
''        If pt1.X > -1 And pt1.Y > -1 And pt1.X < curWImg.Width And pt1.Y < curWImg.Height Then curWImg.SetPixel(pt1.X, pt1.Y, color)
''        Return
''    End If
''    Dim p As New Pen(color, width)
''    g.DrawLine(p, pt1, pt2)
''End Sub

''Public Sub drawCircle(ptC As Point, radius As Integer, color As Color, Optional ByVal width As Integer = 1)
''    Dim p As New Pen(color, width)
''    g.DrawEllipse(p, New Rectangle(ptC.X - radius, ptC.Y - radius, radius * 2, radius * 2))
''End Sub

''Public Sub drawFilledCircle(ptC As Point, radius As Integer, color As Brush)
''    g.FillEllipse(color, New Rectangle(ptC.X - radius, ptC.Y - radius, radius * 2, radius * 2))

''End Sub

''Public Sub drawCross(ByVal pt1 As Point, ByVal color As Color, Optional ByVal width As Integer = 1)
''    drawLine(New Point(0, pt1.Y), New Point(curWImg.Width - 1, pt1.Y), color, width)
''    drawLine(New Point(pt1.X, 0), New Point(pt1.X, curWImg.Height - 1), color, width)

''    drawrect(New Rectangle(pt1.X - 5, pt1.Y - 5, 11, 11), color)
''    drawLine(pt1, pt1, negateColor(color))
''End Sub


''Public Sub drawrect(ByVal rec As Rectangle, ByVal color As Color, Optional ByVal widgthAsPadding As Integer = 1, Optional ByVal widthAsMarging As Integer = 0)
''    Dim pt1 As Point
''    Dim pt2 As Point
''    Dim pt3 As Point
''    Dim pt4 As Point

''    ' to inside
''    For i = 0 To widgthAsPadding - 1

''        pt1.X = rec.X + i
''        pt1.Y = rec.Y + i
''        pt2.X = rec.X + rec.Width - 1 - i
''        pt2.Y = rec.Y + i
''        pt3.X = rec.X + i
''        pt3.Y = rec.Y + rec.Height - 1 - i
''        pt4.X = rec.X + rec.Width - 1 - i
''        pt4.Y = rec.Y + rec.Height - 1 - i

''        drawLine(pt1, pt2, color)
''        drawLine(pt2, pt4, color)
''        drawLine(pt3, pt4, color)
''        drawLine(pt3, pt1, color)
''    Next


''    ' to outside
''    For i = 1 To widthAsMarging
''        pt1.X = rec.X - i
''        pt1.Y = rec.Y - i
''        pt2.X = rec.X + rec.Width - 1 + i
''        pt2.Y = rec.Y - i
''        pt3.X = rec.X - i
''        pt3.Y = rec.Y + rec.Height - 1 + i
''        pt4.X = rec.X + rec.Width - 1 + i
''        pt4.Y = rec.Y + rec.Height - 1 + i

''        drawLine(pt1, pt2, color)
''        drawLine(pt2, pt4, color)
''        drawLine(pt3, pt4, color)
''        drawLine(pt3, pt1, color)
''    Next


''End Sub


''Public Function negateColor(c As Color) As Color
''    Return Color.FromArgb(c.A, 255 - c.R, 255 - c.G, 255 - c.B)
''End Function