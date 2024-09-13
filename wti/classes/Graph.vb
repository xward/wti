Public Class Graph
    ' data source
    Public asset As AssetInfos

    Public parentPanel As Panel

    Public fromDate As Date
    Public toDate As Date
    ' precalcs
    Public spanSec As Double

    Public elapsed As Double

    Private rightPanel As New Panel
    Private leftPanel As New Panel

    Private xLegend As New Panel
    'Private yLegend As New Panel
    Private pictureBox As New PictureBox


    Private labelPrice As New Label

    Private img As New Bitmap(1, 1)
    Private g As Graphics
    Private rendering As Boolean = False

    ' style
    Private defaultFont As New Font("Cascadia Mono", 14)
    Private gridPen As Pen = New Pen(Color.FromArgb(100, Color.Gray))
    Private legendPen As Pen = New Pen(Color.FromArgb(250, Color.Gray))
    Private blackPen As Pen = New Pen(Color.Black)
    Private curvePen As Pen

    Private lastMouseMove As Date = Date.UtcNow
    Private mouseOvering As New Point(-1, -1)


    Public Sub New(parentPanel As Panel, assetName As AssetNameEnum)
        Me.parentPanel = parentPanel
        Me.asset = assetFromName(assetName)
        toDate = Date.UtcNow()
        init()
    End Sub

    'Public Sub New(parentPanel As Panel, asset As AssetInfos)
    '    Me.parentPanel = parentPanel
    '    Me.asset = asset
    '    init()
    'End Sub

    ' mouse over show value, date, diff %
    ' remove nights
    ' how much loss in 5 days, how much loss from max ever(only sp500 credible)
    ' chute intensity graph (=f(% loss, time it took))

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Render
    Public Sub render()
        rendering = True
        Dim start As Date = Date.UtcNow()
        checkResized()
        ' init
        paintItBlack()

        ' precompute stuff
        spanSec = toDate.Subtract(fromDate).TotalSeconds

        ' actual stuff
        renderAssetPrices()

        ' renderOrders()

        ' renderPositions()

        ' renderIndicators()

        pictureBox.Image = img
        Application.DoEvents()
        elapsed = Date.UtcNow.Subtract(start).TotalMilliseconds

        ' temps
        Dim fps As Double = 1000 / Math.Max(1, elapsed)
        FrmMain.ToolStripStatusLabelDrawFps.Text = Math.Round(elapsed) & "ms (" & Math.Round(fps) & " fps)"
        rendering = False
    End Sub


    ' implem AssetHistory = list of assetPrice, that load everything we know on this asset ( only what I recorded, for now ), max 4 month history for now (graph doesn't need more). Use this in simulation instead, implem min/max auto, index 5j auto 3mo auto
    ' implem filter per date min max in assetHistory

    ' find min/max of it percentage
    ' define échelle
    ' fox how much perc horizontal to print

    Private curvePadding As New Padding(5, 50, 85, 70)
    Private curveRect As New Rectangle
    Private minPrice As AssetPrice
    Private maxPrice As AssetPrice
    Private zeroPrice As AssetPrice
    Private history As AssetHistory
    Private allPrices As List(Of AssetPrice)
    Private holesList As New List(Of Hole)

    ' -------------------------------------------------------------------------------------------------------------------------------------------------------------

    Private Sub renderAssetPrices()
        ' current
        Dim price As AssetPrice = getPrice(asset)

        ' might get slow, can be easily optimized
        ' todo: drop outside of market open
        allPrices = history.allPricesAfter(fromDate)
        buildHolesFromAllPrices()




        If allPrices.Count = 0 Then
            writeText(New Point(img.Width / 2 - 30, img.Height / 2 - 5), "No Data", blackPen.Color, Color.Transparent)
            Exit Sub
        End If

        If allPrices.Count = 1 Then
            writeText(New Point(img.Width / 2 - 30, img.Height / 2 - 5), "Not enough Data", blackPen.Color, Color.Transparent)
            Exit Sub
        End If

        zeroPrice = allPrices.ElementAt(0)

        'compute min/max
        minPrice = allPrices.ElementAt(0)
        maxPrice = allPrices.ElementAt(0)
        For Each p As AssetPrice In allPrices
            If p.price < minPrice.price Then minPrice = p
            If p.price > maxPrice.price Then maxPrice = p
        Next
        'g.DrawLine(gridPen, New Point(0, pixelY(minPrice)), New Point(img.Width, pixelY(minPrice)))
        'g.DrawLine(gridPen, New Point(0, pixelY(maxPrice)), New Point(img.Width, pixelY(maxPrice)))

        renderVerticals()


        'For Each h As Hole In holesList
        '    g.DrawLine(New Pen(Color.RebeccaPurple), New Point(pixelX(h.ifAfter), curveRect.Y), New Point(pixelX(h.ifAfter), curveRect.Y + curveRect.Height))


        'Next


        ' horizontal perc separator
        For perc As Double = 0 To -100 Step -2
            Dim y As Double = pixelY(zeroPrice.price * (1.0 + perc / 100))
            If y > curveRect.Y + curveRect.Height Then Exit For
            g.DrawLine(gridPen, New Point(curveRect.X, y), New Point(curveRect.X + curveRect.Width, y))
            writeText(New Point(curveRect.X + curveRect.Width, y), perc & "%", legendPen.Color, Color.Transparent)
        Next
        For perc As Double = 1 To 200 Step 2
            Dim y As Double = pixelY(zeroPrice.price * (1.0 + perc / 100))
            If y < curveRect.Y Then Exit For

            g.DrawLine(gridPen, New Point(curveRect.X, y), New Point(curveRect.X + curveRect.Width, y))
            writeText(New Point(curveRect.X + curveRect.Width, y), perc & "%", legendPen.Color, Color.Transparent)
        Next


        Dim arroyStr As String = ""
        If price.todayChangePerc > 0 Then arroyStr = Convert.ToChar(9650)
        If price.todayChangePerc < 0 Then arroyStr = Convert.ToChar(9660)

        'top text
        writeText(New Point(5, 5), asset.ticker & " - " & asset.name.ToString & " (" & asset.currency & ") " & Math.Round(price.price * 10) / 10 & " " & arroyStr & " " & price.todayChangePerc & "% " &
                      " max_ever:" & Math.Round(history.maxPriceEver.price) & " (" & history.diffWithMaxPerc & "%)", Color.Black, Color.Transparent)
        'sub text
        writeText(New Point(5, 30), "graph min:" & Math.Round(minPrice.price) & " max:" & Math.Round(maxPrice.price) & "   last_point: " & price.dat.ToString, Color.Black, Color.Transparent, 11)

        writeText(New Point(img.Width - 65, 5), allPrices.Count & " pts", Color.Black, Color.Transparent, 11)


        ' asset graph curve itself
        For nu = 1 To allPrices.Count - 1
            Dim nm1 As AssetPrice = allPrices.ElementAt(nu - 1)
            Dim n As AssetPrice = allPrices.ElementAt(nu)

            Dim pt1 As PointF = New PointF(pixelX(nm1), pixelY(nm1))
            Dim pt2 As PointF = New PointF(pixelX(n), pixelY(n))

            g.DrawLine(curvePen, pt1, pt2)
        Next






        'Dim pts As PointF()
        'pts.Append(New PointF(0, 0))

        'g.DrawCurve(blackPen, pts)

        ' on veut draw line 1 by 1

    End Sub

    'drawHorizontalGrid(img.Height / 2, 60)
    'drawVerticalGrid(img.Width / 2, img.Width / (3 * 4))


    ' -----------------------------------------------------------------------------------------------------------------------------
    ' vertical time separators

    'Private stepList As Integer() = {1, 7, 30}
    'Private dayWideStepList As Integer() = {12, 3 * 30, 6 * 30}

    Private Sub renderVerticals()
        Dim stepp As Integer = 1
        ' du pif complet
        If spanSec / curveRect.Width > 100 Then stepp = 5
        If spanSec / curveRect.Width > 5000 Then stepp = 30
        If spanSec / curveRect.Width > 20000 Then stepp = 90

        'dbg.info(spanSec / curveRect.Width)
        'dbg.info(stepp)

        Dim now As Date
        'truncate to day
        If stepp = 5 Then now = Date.Parse(Date.UtcNow.ToShortDateString)
        'truncate to month
        If stepp > 5 Then now = Date.Parse(Date.UtcNow.Month & "/01/" & Date.UtcNow.Year)

        Dim text As String = ""

        For day = 0 To Math.Round(fromDate.Subtract(toDate).TotalDays) Step -stepp
            Dim dat As Date = now.AddDays(day)
            g.DrawLine(gridPen, New Point(pixelX(dat), curveRect.Y), New Point(pixelX(dat), curveRect.Y + curveRect.Height + 20))

            If stepp = 5 Then text = dat.Day
            If stepp > 5 Then text = MonthName(dat.Month, True) & " " & dat.Year

            writeText(New Point(pixelX(dat), curveRect.Y + curveRect.Height + 5), text, legendPen.Color, Color.Transparent, 12)
        Next

    End Sub


    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Holes

    Private Sub buildHolesFromAllPrices()
        'this is terrible !

        holesList.Clear()
        Dim n1 As Date
        Dim n As Date = allPrices.ElementAt(0).dat
        Dim diff As Double

        For nu = 0 To allPrices.Count - 2

            n1 = allPrices.ElementAt(nu + 1).dat

            diff = n1.Subtract(n).TotalSeconds

            n = n1

            'trou de moins de 2h? no hole
            If diff < 120 Then
                n = n1
                Continue For
            End If

            '.begin = n.AddMinutes(1),
            '.endd = n1.AddMinutes(-1),
            holesList.Add(New Hole With {
                .ifAfter = n.AddMinutes(1),
                .shiftSec = Math.Round(diff - 2 * 60)
            })
            n = n1
        Next
        dbg.info(holesList.Count)
        'For Each h As Hole In holesList
        '    dbg.info(StructToString(h))
        'Next
    End Sub

    ' return shift as second to remove
    Private Function shiftFromHoles(x As Date) As Double
        Dim shift As Double = 0
        For Each h As Hole In holesList
            If x.Subtract(h.ifAfter).TotalSeconds > 0 Then shift += h.shiftSec
        Next
        Return 0
    End Function

    Private Function pixelX(price As AssetPrice) As Double
        Return pixelX(price.dat)
    End Function

    'Dim sec As Double = -toDate.Subtract(dat).TotalSeconds

    'Dim minSec As Double = -toDate.Subtract(fromDate).TotalSeconds ' + Math.Floor(toDate.Subtract(fromDate).TotalDays) * (86400 - (asset.marketUTCClose.Subtract(asset.marketOpen).TotalSeconds))
    '' Dim maxSec As Double = 0

    Private Function pixelX(dat As Date) As Double
        Dim timespanSec As Double = toDate.Subtract(fromDate).TotalSeconds
        'dbg.info(timespanSec & " " & dat.Subtract(fromDate).TotalSeconds)
        timespanSec -= shiftFromHoles(toDate)
        'dbg.info(" -> with " & fromDate.ToString & " < " & dat.ToString & " < " & toDate.ToString & "    " & shiftFromHoles(dat))
        'dbg.info("    from shift: " & shiftFromHoles(fromDate) & " datShift:" & shiftFromHoles(dat) & " toShift:" & shiftFromHoles(toDate))

        Dim perVOnGraph As Double = (dat.Subtract(fromDate).TotalSeconds - shiftFromHoles(dat)) / timespanSec
        'dbg.info(" -> " & timespanSec & " " & (dat.Subtract(fromDate).TotalSeconds - shiftFromHoles(dat)) & " -> " & perVOnGraph)

        Return curveRect.Left + perVOnGraph * curveRect.Width
    End Function
    ' dbg.info(minSec & " " & maxSec & " " & " " & sec & " ->" & perVOnGraph)

    ' dbg.info(Math.Floor(dat.Subtract(fromDate).TotalDays) & "  " & perVOnGraph & "  " & (86400 - (asset.marketUTCClose.Subtract(asset.marketOpen).TotalSeconds)))



    Private Function pixelY(price As AssetPrice) As Double
        Return pixelY(price.price)
    End Function
    Private Function pixelY(p As Double) As Double
        ' add padding top/bottom, graph should never touch max échelle
        Dim innerPadding As Integer = 50

        Dim minP As Double = minPrice.price
        If minP = 0 Then minP = p
        Dim maxP As Double = maxPrice.price
        If maxP = 0 Then maxP = p

        Dim perHOnGraph As Double = (p - minP) / (maxP - minP)
        ' dbg.info(minP & " " & maxP & " " & " " & price.price & " ->" & perHOnGraph)


        ' dbg.info(curveRect.Top + innerPadding + (1 - perHOnGraph) * (curveRect.Height - innerPadding * 2))

        Return curveRect.Top + innerPadding + (1 - perHOnGraph) * (curveRect.Height - innerPadding * 2)
    End Function

    'sec between dat and fromDate removing seconds during market close
    Private Function diffFromWithDateSec(dat As Date) As Double

        ''Dim sec As Double = -toDate.Subtract(dat).TotalSeconds

        ''Dim minSec As Double = -toDate.Subtract(fromDate).TotalSeconds ' + Math.Floor(toDate.Subtract(fromDate).TotalDays) * (86400 - (asset.marketUTCClose.Subtract(asset.marketOpen).TotalSeconds))
        ''' Dim maxSec As Double = 0

        'Dim perVOnGraph As Double = diffFromWithDateSec(dat) / diffFromWithDateSec(Date.UtcNow())
        '' dbg.info(minSec & " " & maxSec & " " & " " & sec & " ->" & perVOnGraph)

        '' dbg.info(Math.Floor(dat.Subtract(fromDate).TotalDays) & "  " & perVOnGraph & "  " & (86400 - (asset.marketUTCClose.Subtract(asset.marketOpen).TotalSeconds)))

        'Return curveRect.Left + perVOnGraph * curveRect.Width


        'dbg.info(Math.Floor(dat.Subtract(fromDate).TotalDays))
        Return dat.Subtract(fromDate).TotalSeconds ' - (0 + Math.Floor(dat.Subtract(fromDate).TotalDays)) * 54900
        '            (Math.Floor(toDate.Subtract(dat).TotalDays) - 0) * (86400 - (asset.marketUTCClose.Subtract(asset.marketOpen).TotalSeconds))
    End Function

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Utils

    Private Sub sMoney(price)

    End Sub

    Private Sub checkResized()
        ' todo
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
    Private Sub init()

        fromDate = Date.UtcNow.AddDays(-2000)
        ' truncate from date
        'fromDate = Date.Parse(fromDate.ToShortDateString).AddDays(-5)

        'dbg.info(fromDate)

        'With rightPanel
        '    .Parent = parentPanel
        '    .Width = 75
        '    .Dock = DockStyle.Right
        '    .BackColor = Color.Red '  Color.FromArgb(255, 15, 15, 15)
        'End With

        'With labelPrice
        '    .Parent = rightPanel
        '    .Left = 0
        '    .Top = 0
        '    .Font = New Font("Calibri", 14)
        'End With

        'With leftPanel
        '    .Parent = parentPanel
        '    .Dock = DockStyle.Fill
        '    '.BackColor = Color.Red
        'End With

        '  dock marche pas, go manual
        'With xLegend
        '    .Parent = leftPanel
        '    .Top = leftPanel.Height - 50
        '    .Height = 50
        '    .Left = 0
        '    .Width = leftPanel.Width
        '    .BackColor = Color.FromArgb(255, 15, 15, 15)

        'End With

        With pictureBox
            .Parent = parentPanel
            .Top = 0
            .Left = 0
            .Height = parentPanel.Height
            .Width = parentPanel.Width
            .BackColor = Color.Red
        End With

        curveRect.X = curvePadding.Left
        curveRect.Y = curvePadding.Top
        curveRect.Height = pictureBox.Height - curvePadding.Vertical
        curveRect.Width = pictureBox.Width - curvePadding.Horizontal



        AddHandler pictureBox.MouseMove, AddressOf moveOverGraph

        curvePen = New Pen(asset.lineColor, 1)


        history = getAssetHistory(asset)

        Application.DoEvents()

        render()
    End Sub
    Public Sub setDateRange(fromDate As Date, toDate As Date)
        Me.fromDate = fromDate
        Me.toDate = toDate
        render()
    End Sub

    Public Sub moveOverGraph(sender As Object, e As MouseEventArgs)
        ' 10 fps on mouse over max
        If Date.UtcNow.Subtract(lastMouseMove).TotalMilliseconds < 100 Or rendering Then Exit Sub
        lastMouseMove = Date.UtcNow

        mouseOvering.X = e.X
        mouseOvering.Y = e.Y

        render()
        ' rerender?

    End Sub

    Public Structure Hole
        Dim begin As Date
        Dim endd As Date
        Dim ifAfter As Date
        Dim shiftSec As Double
    End Structure

End Class
' draw back shadow night
'If toDate.Subtract(fromDate).TotalDays < 8 Then
'' todo: manage weekend

'Dim f As Date = Date.Parse(Date.UtcNow.ToShortDateString & " " & asset.marketUTCClose.Hour & ":" & asset.marketUTCClose.Minute & ":00")
'Dim t As Date = Date.Parse(Date.UtcNow.ToShortDateString & " " & asset.marketOpen.Hour & ":" & asset.marketOpen.Minute & ":00")

'For day As Integer = 0 To 1

'g.DrawRectangle(blackPen, New Rectangle(pixelX(f.AddDays(-day)), curveRect.Y, pixelX(t.AddDays(-day)), curveRect.Y + curveRect.Height))
'Next

'End If