Public Class Graph
    ' data source
    Public asset As AssetInfos

    Public parentPanel As Panel
    Public fromDate As Date
    Public toDate As Date

    Public elapsed As Double

    Private rightPanel As New Panel
    Private leftPanel As New Panel

    Private xLegend As New Panel
    'Private yLegend As New Panel
    Private pictureBox As New PictureBox


    Private labelPrice As New Label

    Private img As New Bitmap(1, 1)
    Private g As Graphics

    ' style
    Private defaultFont As New Font("Calibri", 14)
    Private gridPen As Pen = New Pen(Color.FromArgb(100, Color.Gray))
    Private blackPen As Pen = New Pen(Color.Black)


    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Render
    Public Sub render()
        Dim start As Date = Date.UtcNow()
        checkResized()
        ' init
        paintItBlack()

        ' actual stuff
        renderAssetPrices()

        ' renderOrders()

        ' renderPositions()

        ' renderIndicators()

        pictureBox.Image = img
        Application.DoEvents()
        elapsed = Date.UtcNow.Subtract(start).TotalMilliseconds
    End Sub


    Private Sub renderAssetPrices()
        ' current
        Dim price As AssetPrice = getPrice(asset)
        ' history
        Dim history As AssetHistory = getAssetHistory(asset)



        ' might get slow, can be easily optimized
        Dim allPrices As List(Of AssetPrice) = history.allPricesAfter(fromDate)

        Dim minPrice As AssetPrice = allPrices.ElementAt(0)
        Dim maxPrice As AssetPrice = allPrices.ElementAt(0)

        For Each p As AssetPrice In allPrices
            If p.price < minPrice.price Then minPrice = p
            If p.price > maxPrice.price Then maxPrice = p
        Next


        ' implem AssetHistory = list of assetPrice, that load everything we know on this asset ( only what I recorded, for now ), max 4 month history for now (graph doesn't need more). Use this in simulation instead, implem min/max auto, index 5j auto 3mo auto
        ' implem filter per date min max in assetHistory

        ' find min/max of it percentage
        ' define échelle

        drawHorizontalGrid(img.Height / 2, 60)
        drawVerticalGrid(img.Width / 2, img.Width / (3 * 4))


        labelPrice.Text = price.price


        writeText(New Point(100, 100), allPrices.Count, Color.Black, Color.Transparent)

        'Dim pts As PointF()
        'pts.Append(New PointF(0, 0))

        'g.DrawCurve(blackPen, pts)

        ' on veut draw line 1 by 1

    End Sub


    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Utils

    Private Sub checkResized()
        If pictureBox.Size.ToString() <> img.Size.ToString() Then
            img = New Bitmap(pictureBox.Size.Width, pictureBox.Size.Height)
            g = Graphics.FromImage(img)
        End If

    End Sub

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Drawing

    Private Sub paintItBlack()
        g.FillRectangle(New SolidBrush(Color.FromArgb(255, 253, 241, 230)), New Rectangle(0, 0, img.Width, img.Height))
    End Sub

    ' Color.BlanchedAlmond 
    ' Color.FromArgb(255, 250, 237, 220)

    Private Sub drawHorizontalGrid(zeroY As Integer, stepp As Integer)
        For y = zeroY To img.Height Step stepp
            g.DrawLine(gridPen, New Point(0, y), New Point(img.Width, y))
        Next
        For y = zeroY - stepp To 0 Step -stepp
            g.DrawLine(gridPen, New Point(0, y), New Point(img.Width, y))
        Next
    End Sub

    Private Sub drawVerticalGrid(zeroX As Integer, stepp As Integer)
        For x = zeroX To img.Width Step stepp
            g.DrawLine(gridPen, New Point(x, 0), New Point(x, img.Height))
        Next
        For x = zeroX - stepp To 0 Step -stepp
            g.DrawLine(gridPen, New Point(x, 0), New Point(x, img.Height))
        Next
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

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Api
    Public Sub init(parentPanel As Panel, asset As AssetInfos)
        Me.parentPanel = parentPanel
        Me.asset = asset

        fromDate = Date.UtcNow.AddDays(-5)
        toDate = Date.UtcNow

        With rightPanel
            .Parent = parentPanel
            .Width = 75
            .Dock = DockStyle.Right
            .BackColor = Color.FromArgb(255, 15, 15, 15)
        End With

        With labelPrice
            .Parent = rightPanel
            .Left = 0
            .Top = 0
            .Font = New Font("Calibri", 14)
        End With

        With leftPanel
            .Parent = parentPanel
            .Dock = DockStyle.Fill
            '.BackColor = Color.Red
        End With

        '  dock marche pas, go manual
        With xLegend
            .Parent = leftPanel
            .Top = leftPanel.Height - 50
            .Height = 50
            .Left = 0
            .Width = leftPanel.Width
            .BackColor = Color.FromArgb(255, 15, 15, 15)

        End With

        With pictureBox
            .Parent = leftPanel
            .Top = 0
            .Left = 0
            .Height = leftPanel.Height - 50
            .Width = leftPanel.Width
            .BackColor = Color.Red
        End With

        Application.DoEvents()

        render()
    End Sub
    Public Sub setDateRange(fromDate As Date, toDate As Date)
        Me.fromDate = fromDate
        Me.toDate = toDate
        render()
    End Sub

End Class
