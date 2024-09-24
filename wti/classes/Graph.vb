Imports System.Runtime.InteropServices.JavaScript.JSType

Public Class Graph
    ' data source
    Public asset As AssetInfos

    ' Layout
    Public parentPanel As Panel
    'Private rightPanel As New Panel
    'Private leftPanel As New Panel
    'Private xLegend As New Panel
    'Private yLegend As New Panel
    'Private labelPrice As New Label
    Private pictureBox As New PictureBox

    ' dates
    Public defaultFromDate As Date
    Public defaultToDate As Date
    Public fromDate As Date
    Public toDate As Date

    ' precalcs
    Public spanSec As Double
    Public priceUnderMouse As AssetPrice


    'picture
    Private img As New Bitmap(1, 1)
    Private g As Graphics
    Private rendering As Boolean = False
    Private renderRequest As Boolean = False
    Private renderCount As Integer = 0
    Private asyncRenderTimer As Timer

    Public elapsed As Double

    ' style
    Private defaultFont As New Font("Cascadia Mono", 14)
    Private gridPen As Pen = New Pen(Color.FromArgb(100, Color.Gray))
    Private crossdPen As Pen = New Pen(Color.FromArgb(180, Color.Black))
    Private legendPen As Pen = New Pen(Color.FromArgb(250, Color.Gray))
    Private blackPen As Pen = New Pen(Color.Black)
    Private curvePen As Pen

    'interactions:
    ' [done] ctrl molette: scroll x
    ' [done] molette: zoom x
    ' [done] right click: reset all zoom
    ' [mouai] ctrl right click: menu contextuel (when neeed)
    ' [mouai] drag and drop zoom specific time frame
    ' [done] mouse over cross with metadata, also show diagonale any usefull kpi

    ' todo:
    ' 1 days, 5 days, 1mo, 3 mo, 6 months 1 ytd 1y 3y 5y MAX
    ' vertical position in, position out
    ' build list of aggregage of prices, and use that instead (like a candle)
    ' order, positions, past trades

    ' how much loss in 5 days
    ' [done] how much loss from max ever(only sp500 credible)
    ' chute intensity graph (=f(% loss, time it took))

    Public Sub New(parentPanel As Panel, assetName As AssetNameEnum)
        Me.parentPanel = parentPanel
        Me.asset = assetFromName(assetName)
        toDate = Date.UtcNow()
        init()

        asyncRenderTimer = New Timer
        asyncRenderTimer.Interval = 10
        AddHandler asyncRenderTimer.Tick, AddressOf AsyncRenderTimer_Tick
        asyncRenderTimer.Enabled = True
    End Sub

    Private Sub AsyncRenderTimer_Tick(sender As Object, e As EventArgs)
        asyncRenderTimer.Enabled = False
        If renderRequest And Not rendering Then
            render()
            renderRequest = False
        End If
        If status <> StatusEnum.INTERRUPT Then asyncRenderTimer.Enabled = True
    End Sub

    ' bugs
    ' si le zeroprice = 0, ca pete un peu partout
    ' si je zoom sur des donnée avant le fromdate original, ca pète

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Render

    Public Sub asyncRender()
        renderRequest = True
    End Sub

    Public Sub render()
        rendering = True
        renderCount += 1
        Dim start As Date = Date.UtcNow()
        checkResized()
        ' init
        paintItBlack()

        pictureBox.Cursor = Cursors.Cross

        g.DrawRectangle(gridPen, curveRect)

        ' precompute stuff
        If CST.HOST_NAME = whoIsCollecting Then
            ' On ghost, real time data
            defaultToDate = Date.UtcNow
            toDate = Date.UtcNow

            fromDate = toDate.AddDays(-5)
            If fromDate.CompareTo(getAssetHistory(asset).oldestPrice.dat) < 0 Then fromDate = getAssetHistory(asset).oldestPrice.dat
        End If

        spanSec = toDate.Subtract(fromDate).TotalSeconds

        renderDDHorizontals()

        renderButtons()


        ' actual stuff
        renderAssetPrices()

        ' renderOrders()
        ' renderPositions()

        ' ie graph écart max % curve
        ' renderIndicators()
        renderIndicators()

        pictureBox.Image = img
        elapsed = Date.UtcNow.Subtract(start).TotalMilliseconds


        Application.DoEvents()

        ' temps
        Dim fps As Double = 1000 / Math.Max(1, elapsed)
        FrmMain.ToolStripStatusLabelDrawFps.Text = Math.Round(elapsed) & "ms (" & Math.Round(fps) & " fps) #" & renderCount
        rendering = False
    End Sub

    ' -------------------------------------------------------------------------------------------------------------------------------------------------------------
    ' buttons

    Dim buttons As New List(Of GraphButton)
    Dim tmpButton As GraphButton

    Private Sub buildTimeFrameButton(x As Integer, name As String, text As String)
        Dim rect As New Rectangle(0, img.Height - 50, 30, 30)
        rect.X = x

        tmpButton = New GraphButton
        With tmpButton
            .backColor = Color.Transparent
            .borderColor = blackPen.Color
            .mouseOverColor = Color.LightSteelBlue
            .name = name
            .text = text
            .rect = rect
        End With
        buttons.Add(tmpButton)
    End Sub

    Private Sub initButtons()
        buttons.Clear()
        Dim x As Integer = img.Width - 440
        buildTimeFrameButton(x, "Time5D", "5D")
        buildTimeFrameButton(x + 40, "Time1M", "1M")
        buildTimeFrameButton(x + 80, "Time3M", "3M")
        buildTimeFrameButton(x + 120, "Time6M", "6M")
        buildTimeFrameButton(x + 160, "Time1Y", "1Y")
        buildTimeFrameButton(x + 200, "Time3Y", "3Y")
        buildTimeFrameButton(x + 240, "Time6Y", "6Y")
        buildTimeFrameButton(x + 280, "Time15Y", "15Y")
        buildTimeFrameButton(x + 320, "TimeMAX", "MAX")
    End Sub

    Private Sub buttonCallback(x As Integer, y As Integer)
        For Each b As GraphButton In buttons
            If inRect(New Point(x, y), b.rect) Then
                'dbg.info("clic " & b.name)
                Select Case b.name
                    Case "Time5D"
                        toDate = Date.UtcNow
                        fromDate = Date.UtcNow.AddDays(-5)
                    Case "Time1M"
                        toDate = Date.UtcNow
                        fromDate = Date.UtcNow.AddDays(-30)
                    Case "Time3M"
                        toDate = Date.UtcNow
                        fromDate = Date.UtcNow.AddDays(-30 * 3)
                    Case "Time6M"
                        toDate = Date.UtcNow
                        fromDate = Date.UtcNow.AddDays(-30 * 6)
                    Case "Time1Y"
                        toDate = Date.UtcNow
                        fromDate = Date.UtcNow.AddDays(-365)
                    Case "Time3Y"
                        toDate = Date.UtcNow
                        fromDate = Date.UtcNow.AddDays(-365 * 3)
                    Case "Time6Y"
                        toDate = Date.UtcNow
                        fromDate = Date.UtcNow.AddDays(-365 * 6)
                    Case "Time15Y"
                        toDate = Date.UtcNow
                        fromDate = Date.UtcNow.AddDays(-365 * 15)
                    Case "TimeMAX"
                        toDate = Date.UtcNow
                        fromDate = Date.UtcNow.AddDays(-365 * 30)
                    Case Else
                        'nothing
                End Select
            End If
        Next
    End Sub

    Private Sub renderButtons()
        For Each b As GraphButton In buttons
            g.DrawRectangle(New Pen(b.borderColor), b.rect)
            If inRect(mouseOvering, b.rect) Then
                g.FillRectangle(New SolidBrush(b.mouseOverColor), b.rect)
                pictureBox.Cursor = Cursors.Hand
            End If
            writeText(New Point(b.rect.X + 5, b.rect.Y + 9), b.text, b.borderColor, b.backColor, 12)
        Next
    End Sub


    ' -------------------------------------------------------------------------------------------------------------------------------------------------------------
    ' render curve itself


    Private curveRect As New Rectangle

    Private curveDDRect As New Rectangle

    Private minPrice As AssetPrice
    Private maxPrice As AssetPrice
    Private zeroPrice As AssetPrice
    Private history As AssetHistory
    Private allPrices As List(Of AssetPrice)
    Private Sub renderAssetPrices()
        Dim start As Date = Date.UtcNow()

        ' might get slow, can be easily optimized
        ' todo: drop outside of market open
        allPrices = history.allPricesBetween(fromDate, toDate)
        ' buildHolesFromAllPrices()

        Dim eFetch As Double = Date.UtcNow.Subtract(start).TotalMilliseconds


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
        renderHorizontals()
        renderTopText()

        Dim eProcess As Double = Date.UtcNow.Subtract(start).TotalMilliseconds - eFetch


        ' asset graph curve itself
        For nu = 1 To allPrices.Count - 1
            Dim nm1 As AssetPrice = allPrices.ElementAt(nu - 1)
            Dim n As AssetPrice = allPrices.ElementAt(nu)

            Dim pt1 As PointF = New PointF(dateToX(nm1), priceToY(nm1))
            Dim pt2 As PointF = New PointF(dateToX(n), priceToY(n))

            g.DrawLine(curvePen, pt1, pt2)
        Next



        Dim ePlot As Double = Date.UtcNow.Subtract(start).TotalMilliseconds - eProcess

        renderMouseOverCross()


        'Dim pts As PointF()
        'pts.Append(New PointF(0, 0))

        'g.DrawCurve(blackPen, pts)

        ' on veut draw line 1 by 1

        ' dbg.info("Render prices eFetch=" & Math.Round(eFetch) & " eProcess=" & Math.Round(eProcess) & " ePlot=" & Math.Round(ePlot))
    End Sub

    'drawHorizontalGrid(img.Height / 2, 60)
    'drawVerticalGrid(img.Width / 2, img.Width / (3 * 4))

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' drawdown graph

    Private Function curveDDtoY(diff As Double) As Double
        If diff > maxDDGraphPerc Then diff = maxDDGraphPerc
        Return curveDDRect.Y + curveDDRect.Height - diff / 100 * curveDDRect.Height * 100 / 55
    End Function


    Private Sub renderIndicators()

        'curve itself
        For nu = 1 To allPrices.Count - 1
            Dim nm1 As AssetPrice = allPrices.ElementAt(nu - 1)
            Dim n As AssetPrice = allPrices.ElementAt(nu)

            If nm1.currentMaxPrice.price = 0 Then Continue For

            If n.currentMaxPrice.price = 0 Then Continue For

            Dim pt1 As PointF = New PointF(dateToX(nm1), curveDDtoY(nm1.diffFromMaxPrice()))
            Dim pt2 As PointF = New PointF(dateToX(n), curveDDtoY(n.diffFromMaxPrice()))

            g.DrawLine(curvePen, pt1, pt2)
        Next
    End Sub


    ' -----------------------------------------------------------------------------------------------------------------------------
    ' vertical time separators

    'Private stepList As Integer() = {1, 7, 30}
    'Private dayWideStepList As Integer() = {12, 3 * 30, 6 * 30}

    Private Sub renderVerticals()
        'ctp, on voit quand je zoom

        Dim stepp As Integer = 1
        ' du pif complet
        If spanSec / curveRect.Width > 100 Then stepp = 5
        If spanSec / curveRect.Width > 5000 Then stepp = 30
        If spanSec / curveRect.Width > 20000 Then stepp = 90
        If spanSec / curveRect.Width > 50000 Then stepp = 30 * 6
        If spanSec / curveRect.Width > 400000 Then stepp = 365

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
            g.DrawLine(gridPen, New Point(dateToX(dat), curveRect.Y), New Point(dateToX(dat), curveRect.Y + curveRect.Height))

            g.DrawLine(gridPen, New Point(dateToX(dat), curveDDRect.Y), New Point(dateToX(dat), curveDDRect.Y + curveDDRect.Height))

            If stepp = 5 Then text = dat.Day
            If stepp > 5 Then text = MonthName(dat.Month, True) & "" & dat.Year.ToString.Substring(2)

            Dim shiftText As Integer = 25
            If stepp = 5 Then shiftText = 5
            writeText(New Point(dateToX(dat) - shiftText, curveRect.Y + curveRect.Height + 5), text, legendPen.Color, Color.Transparent, 12)
        Next

    End Sub

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' horizontal perc value separators

    Private Sub renderHorizontals()
        Dim zero As Double = zeroPrice.price
        If zero = 0 Then zero = 1

        Dim minPerc As Integer = -Math.Round((1 - minPrice.price / zero) * 100)
        Dim maxPerc As Integer = Math.Round(maxPrice.price / zero * 100)

        For perc As Double = minPerc To maxPerc Step (Math.Abs(maxPerc) - Math.Abs(minPerc)) / 12
            Dim y As Double = priceToY(zero * (1.0 + perc / 100))
            If y < curveRect.Y Or y > curveRect.Y + curveRect.Height Then Continue For

            g.DrawLine(gridPen, New Point(curveRect.X, y), New Point(curveRect.X + curveRect.Width, y))
            writeText(New Point(curveRect.X + curveRect.Width, y - 6), Math.Round(perc) & "%", legendPen.Color, Color.Transparent)
            writeText(New Point(curveRect.X + curveRect.Width + 55, y + 1 - 6), formatPrice(zero * (1 + perc / 100)), legendPen.Color, Color.Transparent, 11)
        Next
    End Sub

    Private maxDDGraphPerc As Double = 55
    Private Sub renderDDHorizontals()
        For perc As Double = 0 To maxDDGraphPerc Step 5
            Dim y As Double = curveDDtoY(perc)

            g.DrawLine(gridPen, New Point(curveDDRect.X, y), New Point(curveDDRect.X + curveDDRect.Width, y))
            writeText(New Point(curveDDRect.X + curveDDRect.Width, y - 6), Math.Round(perc) & "%", legendPen.Color, Color.Transparent)
        Next
    End Sub

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Mouse over cross with tooltips

    Private Sub renderMouseOverCross()
        If mouseOvering.X = -1 Then Exit Sub

        priceUnderMouse = PriceFromX(mouseOvering.X)
        Dim y As Double = priceToY(priceUnderMouse)
        Dim perc As Double = (Math.Round((priceUnderMouse.price / zeroPrice.price - 1) * 100 * 10)) / 10

        ' bottom dirty text
        writeText(New Point(3, img.Height - 16), "UnderMouse " & formatPrice(priceUnderMouse.price) & " max_ever_before " & formatPrice(priceUnderMouse.currentMaxPrice.price) & " " & priceUnderMouse.diffFromMaxPrice & "% below max", Color.Black, Color.Transparent, 13)

        ' vertical on cursor
        g.DrawLine(crossdPen, New Point(mouseOvering.X, curveRect.Y), New Point(mouseOvering.X, curveRect.Y + curveRect.Height))
        g.DrawLine(crossdPen, New Point(mouseOvering.X, curveDDRect.Y), New Point(mouseOvering.X, curveDDRect.Y + curveDDRect.Height))


        ' horizontal on price price plate
        g.FillRectangle(New SolidBrush(Color.Black), New Rectangle(curveRect.X + curveRect.Width, y - 12, 150, 25))
        writeText(New Point(curveRect.X + curveRect.Width, y - 12 + 6), perc.ToString("#0.0") & "%", Color.White, Color.Transparent)
        writeText(New Point(curveRect.X + curveRect.Width + 75, y - 12 + 8), formatPrice(priceUnderMouse.price), Color.White, Color.Transparent, 11)

        ' curveDD: horizontal on price diff from max price 
        g.FillRectangle(New SolidBrush(Color.Black), New Rectangle(curveDDRect.X + curveDDRect.Width, curveDDtoY(priceUnderMouse.diffFromMaxPrice) - 12, 150, 25))
        writeText(New Point(curveDDRect.X + curveDDRect.Width, curveDDtoY(priceUnderMouse.diffFromMaxPrice) - 12 + 6), priceUnderMouse.diffFromMaxPrice.ToString("#0.0") & "%", Color.White, Color.Transparent)


        If inRect(mouseOvering, curveRect) Then
            ' horizontal on cursor
            g.DrawLine(crossdPen, New Point(curveRect.X, mouseOvering.Y), New Point(curveRect.X + curveRect.Width, mouseOvering.Y))

            ' horizontal on price
            ' g.DrawLine(crossdPen, New Point(curveRect.X, y), New Point(curveRect.X + curveRect.Width, y))

            ' horizontal on cursor price plate
            Dim percUnderMouse As Double = (Math.Round((yToPrice(mouseOvering.Y) / zeroPrice.price - 1) * 100 * 10)) / 10
            g.FillRectangle(New SolidBrush(Color.DarkGray), New Rectangle(curveRect.X + curveRect.Width, mouseOvering.Y - 5, 150, 25))
            writeText(New Point(curveRect.X + curveRect.Width, mouseOvering.Y - 5 + 6), percUnderMouse.ToString("#0.0") & "%", Color.White, Color.Transparent)
            writeText(New Point(curveRect.X + curveRect.Width + 75, mouseOvering.Y - 5 + 8), formatPrice(yToPrice(mouseOvering.Y)), Color.White, Color.Transparent, 11)
        End If

        If inRect(mouseOvering, curveDDRect) Then
            ' curveDD: horizontal on cursor
            g.DrawLine(crossdPen, New Point(curveDDRect.X, mouseOvering.Y), New Point(curveDDRect.X + curveDDRect.Width, mouseOvering.Y))



        End If


        If mouseOvering.X > curveRect.X And mouseOvering.X < curveRect.X + curveRect.Width Then
            ' vertial on price date plate
            Dim shift As Integer = 92
            Dim datePlateX As Integer = mouseOvering.X - shift - 5
            If datePlateX < 0 Then datePlateX = 0
            g.FillRectangle(New SolidBrush(Color.Black), New Rectangle(datePlateX, curveRect.Y + curveRect.Height, shift * 2 + 5 * 2, 25))
            writeText(New Point(datePlateX + 5, curveRect.Y + curveRect.Height + 6), priceUnderMouse.dat.ToString("dd/MM/yyyy HH:mm:ss"), Color.White, Color.Transparent, 12)


        End If

    End Sub

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' top text
    Private Sub renderTopText()
        ' current
        Dim price As AssetPrice = getPrice(asset)

        Dim arroyStr As String = ""
        If price.todayChangePerc > 0 Then arroyStr = Convert.ToChar(9650)
        If price.todayChangePerc < 0 Then arroyStr = Convert.ToChar(9660)

        'top text
        writeText(New Point(5, 5), asset.ticker & " - " & asset.name.ToString & " (" & asset.currency & ") " & formatPrice(price.price) & " " & arroyStr & " " & price.todayChangePerc & "% " &
                          " max:" & formatPrice(price.currentMaxPrice.price) & " (" & price.diffFromMaxPrice & "%)", Color.Black, Color.Transparent)
        'sub text
        writeText(New Point(5, 30), "graph min:" & formatPrice(minPrice.price) & " max:" & formatPrice(maxPrice.price) & "   last_point: " & price.dat.ToString, Color.Black, Color.Transparent, 11)


        writeText(New Point(img.Width - 65, 5), allPrices.Count & " pts", Color.Black, Color.Transparent, 11)
        writeText(New Point(img.Width - 125, 18), "max_ever " & formatPrice(history.maxPriceEver.price), Color.Black, Color.Transparent, 11)
    End Sub

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Date <> Pixel | Perc <> Pixel

    Private Function dateToX(price As AssetPrice) As Double
        Return dateToX(price.dat)
    End Function

    'Dim sec As Double = -toDate.Subtract(dat).TotalSeconds

    'Dim minSec As Double = -toDate.Subtract(fromDate).TotalSeconds ' + Math.Floor(toDate.Subtract(fromDate).TotalDays) * (86400 - (asset.marketUTCClose.Subtract(asset.marketOpen).TotalSeconds))
    '' Dim maxSec As Double = 0

    Private Function dateToX(dat As Date) As Double
        Dim timespanSec As Double = toDate.Subtract(fromDate).TotalSeconds
        'dbg.info(timespanSec & " " & dat.Subtract(fromDate).TotalSeconds)
        ' timespanSec -= shiftFromHoles(toDate)
        'dbg.info(" -> with " & fromDate.ToString & " < " & dat.ToString & " < " & toDate.ToString & "    " & shiftFromHoles(dat))
        'dbg.info("    from shift: " & shiftFromHoles(fromDate) & " datShift:" & shiftFromHoles(dat) & " toShift:" & shiftFromHoles(toDate))

        '  Dim perVOnGraph As Double = (dat.Subtract(fromDate).TotalSeconds - shiftFromHoles(dat)) / timespanSec

        Dim perVOnGraph As Double = dat.Subtract(fromDate).TotalSeconds / timespanSec

        'dbg.info(" -> " & timespanSec & " " & (dat.Subtract(fromDate).TotalSeconds - shiftFromHoles(dat)) & " -> " & perVOnGraph)

        Return curveRect.Left + perVOnGraph * curveRect.Width
    End Function

    Private Function xToDate(x As Double) As Date
        Dim timespanSec As Double = toDate.Subtract(fromDate).TotalSeconds
        Dim perVOnGraph As Double = (x - curveRect.Left) / curveRect.Width

        Dim secFromFromDate As Double = perVOnGraph * timespanSec

        Return fromDate.AddSeconds(secFromFromDate)
    End Function

    ' dbg.info(minSec & " " & maxSec & " " & " " & sec & " ->" & perVOnGraph)
    ' dbg.info(Math.Floor(dat.Subtract(fromDate).TotalDays) & "  " & perVOnGraph & "  " & (86400 - (asset.marketUTCClose.Subtract(asset.marketOpen).TotalSeconds)))

    Private Function priceToY(price As AssetPrice) As Double
        Return priceToY(price.price)
    End Function

    ' add padding top/bottom, graph should never touch max echelle
    Dim yInnerPadding As Integer = 50
    Private Function priceToY(p As Double) As Double

        Dim minP As Double = minPrice.price
        If minP = 0 Then minP = p
        Dim maxP As Double = maxPrice.price
        If maxP = 0 Then maxP = p

        Dim perHOnGraph As Double = (p - minP) / (maxP - minP)
        ' dbg.info(minP & " " & maxP & " " & " " & price.price & " ->" & perHOnGraph)

        ' dbg.info(curveRect.Top + innerPadding + (1 - perHOnGraph) * (curveRect.Height - innerPadding * 2))

        Return curveRect.Top + yInnerPadding + (1 - perHOnGraph) * (curveRect.Height - yInnerPadding * 2)
    End Function

    Private Function yToPrice(y As Double) As Double
        Dim perHOnGraph As Double = -((y - curveRect.Top - yInnerPadding) / (curveRect.Height - yInnerPadding * 2) - 1)

        Dim minP As Double = minPrice.price
        Dim maxP As Double = maxPrice.price

        Return perHOnGraph * (maxP - minP) + minP
    End Function


    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Utils
    Private Sub checkResized()
        If parentPanel.Size.ToString() <> img.Size.ToString() Then
            img = New Bitmap(pictureBox.Size.Width, pictureBox.Size.Height)
            g = Graphics.FromImage(img)
            setDrawBoxes()
        End If
    End Sub



    Private Function PriceFromX(x As Integer) As AssetPrice
        ' find closest asset price from pixel x (mouse over...)
        Dim mouseDate As Date = xToDate(x)
        Dim lowerI As Integer = 0
        Dim upI As Integer = allPrices.Count - 1

        While upI - lowerI > 1
            Dim mid As Integer = Math.Round((upI + lowerI) / 2)
            Dim p As AssetPrice = allPrices.ElementAt(mid)
            If mouseDate.CompareTo(p.dat) > 0 Then lowerI = mid Else upI = mid
            'Application.DoEvents()
        End While

        If Math.Abs(mouseDate.Subtract(allPrices.ElementAt(upI).dat).TotalMilliseconds) > Math.Abs(mouseDate.Subtract(allPrices.ElementAt(lowerI).dat).TotalMilliseconds) Then
            Return allPrices.ElementAt(lowerI)
        Else
            Return allPrices.ElementAt(upI)
        End If
    End Function

    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Drawing

    Private Sub paintItBlack()
        g.FillRectangle(New SolidBrush(Color.FromArgb(255, 253, 241, 230)), New Rectangle(0, 0, img.Width, img.Height))
    End Sub

    ' Color.BlanchedAlmond
    ' Color.FromArgb(255, 250, 237, 220)

    'Private Sub drawHorizontalGrid(zeroY As Integer, stepp As Integer)
    '    For y = zeroY To img.Height Step stepp
    '        g.DrawLine(gridPen, New Point(0, y), New Point(img.Width, y))
    '    Next
    '    For y = zeroY - stepp To 0 Step -stepp
    '        g.DrawLine(gridPen, New Point(0, y), New Point(img.Width, y))
    '    Next
    'End Sub

    'Private Sub drawVerticalGrid(zeroX As Integer, stepp As Integer)
    '    For x = zeroX To img.Width Step stepp
    '        g.DrawLine(gridPen, New Point(x, 0), New Point(x, img.Height))
    '    Next
    '    For x = zeroX - stepp To 0 Step -stepp
    '        g.DrawLine(gridPen, New Point(x, 0), New Point(x, img.Height))
    '    Next
    'End Sub

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
    ' init at create
    Private Sub init()

        If asset.name = AssetNameEnum.SP500 Then
            fromDate = Date.UtcNow.AddYears(-10)
        Else
            fromDate = Date.UtcNow.AddDays(-30)
        End If

        If fromDate.CompareTo(getAssetHistory(asset).oldestPrice.dat) < 0 Then fromDate = getAssetHistory(asset).oldestPrice.dat


        defaultFromDate = fromDate
        defaultToDate = toDate

        crossdPen.DashStyle = Drawing2D.DashStyle.Dot

        With pictureBox
            .Parent = parentPanel
            .Top = 0
            .Left = 0
            .Height = parentPanel.Height
            .Width = parentPanel.Width
            .BackColor = Color.FromArgb(255, 253, 241, 230)
            .Dock = DockStyle.Fill
            .Cursor = Cursors.Cross
            '.ContextMenuStrip = FrmMain.ContextMenuStripGraph
        End With

        setDrawBoxes()

        AddHandler pictureBox.MouseMove, AddressOf moveOverGraph
        AddHandler pictureBox.MouseWheel, AddressOf mouseWheelOnGraph
        AddHandler pictureBox.MouseClick, AddressOf mouseClickOnGraph
        AddHandler pictureBox.MouseLeave, AddressOf mouseLeaveGraph


        curvePen = New Pen(asset.lineColor, 1)


        history = getAssetHistory(asset)



        Application.DoEvents()

        render()
    End Sub

    Private Sub setDrawBoxes()
        Dim topCurveBox As New Rectangle(0, 0, pictureBox.Width, pictureBox.Height / 2 - 1)
        Dim curvePadding As New Padding(5, 50, 85 + 45, 70)

        curveRect.X = topCurveBox.X + curvePadding.Left
        curveRect.Y = topCurveBox.Y + curvePadding.Top
        curveRect.Height = topCurveBox.Height - curvePadding.Vertical
        curveRect.Width = topCurveBox.Width - curvePadding.Horizontal

        Dim drawDownDownCurveBox As New Rectangle(0, pictureBox.Height / 2, pictureBox.Width, pictureBox.Height / 2)

        curveDDRect = New Rectangle(drawDownDownCurveBox.X + curvePadding.Left, drawDownDownCurveBox.Y + curvePadding.Top, drawDownDownCurveBox.Width - curvePadding.Horizontal, drawDownDownCurveBox.Height - curvePadding.Vertical)


        initButtons()

    End Sub


    ' -----------------------------------------------------------------------------------------------------------------------------
    ' Interraction

    Private lastMouseMove As Date = Date.UtcNow
    Private mouseOvering As New Point(-1, -1)

    Private startDragAndDropX As Integer = -1
    Private dragAndDropScrollXFromDate As Date
    Private dragAndDropScrollXToDate As Date
    Private dragAndDropHourPerPixel As Double

    Public Sub moveOverGraph(sender As Object, e As MouseEventArgs)
        '  Dim spanHours As Double = toDate.Subtract(fromDate).TotalHours


        If e.Button = MouseButtons.Left Then
            'init dnd
            If startDragAndDropX = -1 Then
                startDragAndDropX = e.X
                dragAndDropScrollXFromDate = fromDate
                dragAndDropScrollXToDate = toDate
                Dim spanHours As Double = toDate.Subtract(fromDate).TotalHours
                dragAndDropHourPerPixel = spanHours / curveRect.Width
                ' i wanted grab hand closed
                pictureBox.Cursor = Cursors.VSplit
            End If



            Dim shift As Integer = -dragAndDropHourPerPixel * (e.X - startDragAndDropX)

            ' If fromDate.CompareTo(defaultFromDate) < 0 And shift < 0 Then fromDate = defaultFromDate.AddDays(-5)
            If dragAndDropScrollXToDate.AddHours(shift).CompareTo(defaultToDate) > 0 Then
                toDate = defaultToDate
                Exit Sub
            End If

            fromDate = dragAndDropScrollXFromDate.AddHours(shift)
            toDate = dragAndDropScrollXToDate.AddHours(shift)
        Else

            ' 20 fps on mouse over max
            ' If Date.UtcNow.Subtract(lastMouseMove).TotalMilliseconds < 60 Or rendering Then Exit Sub

            ' lastMouseMove = Date.UtcNow
            mouseOvering.X = e.X
            mouseOvering.Y = e.Y

        End If
        asyncRender()
    End Sub

    Public Sub mouseLeaveGraph(sender As Object, e As EventArgs)
        mouseOvering.X = -1
        mouseOvering.Y = -1
        render()
    End Sub

    Public Sub mouseWheelOnGraph(sender As Object, e As MouseEventArgs)
        If rendering Then Exit Sub

        ' scroll x
        If My.Computer.Keyboard.CtrlKeyDown Then
            Dim spanHours As Double = toDate.Subtract(fromDate).TotalHours

            Dim speed As Double = 0.005
            If My.Computer.Keyboard.ShiftKeyDown Then speed = 0.03

            ' 7%
            Dim shift As Integer = spanHours * speed

            If e.Delta > 0 Then shift = -shift
            fromDate = fromDate.AddHours(shift)
            toDate = toDate.AddHours(shift)
            render()
        Else
            ' zoom
            Dim spanHours As Double = toDate.Subtract(fromDate).TotalHours
            ' remove/add 10% of timespan
            Dim shift As Integer = spanHours * 0.1
            If e.Delta < 0 Then shift = -shift
            Dim ratio As Double = priceUnderMouse.dat.Subtract(fromDate).TotalHours / spanHours

            'this is nice, but we add 5 day to show there is nothing more after
            fromDate = fromDate.AddHours(ratio * shift)
            If fromDate.CompareTo(defaultFromDate) < 0 Then fromDate = defaultFromDate.AddDays(-5)
            toDate = toDate.AddHours(-(1 - ratio) * shift)
            If toDate.CompareTo(defaultToDate) > 0 Then toDate = defaultToDate.AddDays(5)
            render()
        End If
    End Sub



    Public Sub mouseClickOnGraph(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            ' startDragAndDropX = e.X
            ' end drag and drop
            startDragAndDropX = -1
            pictureBox.Cursor = Cursors.Cross
        End If

        If e.Button = MouseButtons.Left Then buttonCallback(e.X, e.Y)

        If e.Button = MouseButtons.Right And Not My.Computer.Keyboard.CtrlKeyDown Then
            fromDate = defaultFromDate
            toDate = defaultToDate
            render()
        End If

        'If e.Button = MouseButtons.Right And My.Computer.Keyboard.CtrlKeyDown Then
        '    FrmMain.ContextMenuStripGraph.Top = e.Y
        '    FrmMain.ContextMenuStripGraph.Left = e.X
        '    FrmMain.ContextMenuStripGraph.Visible = True
        'End If
    End Sub


End Class

Public Structure GraphButton
    Dim rect As Rectangle
    Dim borderColor As Color
    Dim backColor As Color
    Dim mouseOverColor As Color
    Dim name As String
    Dim text As String
End Structure

' draw back shadow night
'If toDate.Subtract(fromDate).TotalDays < 8 Then
'' todo: manage weekend

'Dim f As Date = Date.Parse(Date.UtcNow.ToShortDateString & " " & asset.marketUTCClose.Hour & ":" & asset.marketUTCClose.Minute & ":00")
'Dim t As Date = Date.Parse(Date.UtcNow.ToShortDateString & " " & asset.marketOpen.Hour & ":" & asset.marketOpen.Minute & ":00")

'For day As Integer = 0 To 1

'g.DrawRectangle(blackPen, New Rectangle(pixelX(f.AddDays(-day)), curveRect.Y, pixelX(t.AddDays(-day)), curveRect.Y + curveRect.Height))
'Next

'End If

'sec between dat and fromDate removing seconds during market close
'Private Function diffFromWithDateSec(dat As Date) As Double

'    ''Dim sec As Double = -toDate.Subtract(dat).TotalSeconds

'    ''Dim minSec As Double = -toDate.Subtract(fromDate).TotalSeconds ' + Math.Floor(toDate.Subtract(fromDate).TotalDays) * (86400 - (asset.marketUTCClose.Subtract(asset.marketOpen).TotalSeconds))
'    ''' Dim maxSec As Double = 0

'    'Dim perVOnGraph As Double = diffFromWithDateSec(dat) / diffFromWithDateSec(Date.UtcNow())
'    '' dbg.info(minSec & " " & maxSec & " " & " " & sec & " ->" & perVOnGraph)

'    '' dbg.info(Math.Floor(dat.Subtract(fromDate).TotalDays) & "  " & perVOnGraph & "  " & (86400 - (asset.marketUTCClose.Subtract(asset.marketOpen).TotalSeconds)))

'    'Return curveRect.Left + perVOnGraph * curveRect.Width


'    'dbg.info(Math.Floor(dat.Subtract(fromDate).TotalDays))
'    Return dat.Subtract(fromDate).TotalSeconds ' - (0 + Math.Floor(dat.Subtract(fromDate).TotalDays)) * 54900
'    '            (Math.Floor(toDate.Subtract(dat).TotalDays) - 0) * (86400 - (asset.marketUTCClose.Subtract(asset.marketOpen).TotalSeconds))
'End Function
