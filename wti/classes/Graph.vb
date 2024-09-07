Public Class Graph
    ' data source
    Public asset As AssetInfos

    Public parentPanel As Panel
    Public fromDate As Date
    Public toDate As Date

    Public elapsed As Double

    Private xLegend As New Panel
    Private yLegend As New Panel
    Private pictureBox As New PictureBox

    Private img As New Bitmap(1, 1)
    Private g As Graphics

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

        drawHorizontalGrid(img.Height / 2, 60)
        drawVerticalGrid(img.Width / 2, img.Width / (3 * 4))

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


    Private gridPen As Pen = New Pen(Color.FromArgb(100, Color.Gray))


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

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Api
    Public Sub init(parentPanel As Panel, asset As AssetInfos)
        Me.parentPanel = parentPanel
        Me.asset = asset

        fromDate = Date.UtcNow.AddDays(-5)
        toDate = Date.UtcNow

        With yLegend
            .Parent = parentPanel
            .Width = 75
            .Dock = DockStyle.Right
            .BackColor = Color.Green
        End With

        With xLegend
            .Parent = parentPanel
            .Dock = DockStyle.Bottom
            .Height = 50
            .BackColor = Color.Yellow
        End With

        With pictureBox
            .Parent = parentPanel
            .Enabled = True
            .Dock = DockStyle.Fill
            .BackColor = Color.Red
        End With

        render()
    End Sub
    Public Sub setDateRange(fromDate As Date, toDate As Date)
        Me.fromDate = fromDate
        Me.toDate = toDate
        render()
    End Sub

End Class
