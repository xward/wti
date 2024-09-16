Imports System.Globalization
Imports System.IO

Public Class AnalyzeViewer

    Private img As New Bitmap(1, 1)
    Private g As Graphics

    Private Shared dfi = DateTimeFormatInfo.CurrentInfo.CalendarWeekRule

    Private Sub AnalyzeViewer_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        img = New Bitmap(PictureBoxAnalizeViewer.Width, PictureBoxAnalizeViewer.Height)
        g = Graphics.FromImage(img)
        PictureBoxAnalizeViewer.Image = img
        Application.DoEvents()
    End Sub

    ' ------------------------------------------------------------------------------------------------------------------
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim start As Date = Date.UtcNow
        Dim asset As AssetInfos = FrmMain.mainGraph.asset
        Dim assetHistory As AssetHistory = getAssetHistory(asset)

        paintItBlack()
        Dim prices As List(Of AssetPrice) = assetHistory.allPricesBetween(Date.Parse("01/01/1981"), Date.UtcNow)

        Dim timespanDays As Double = Date.UtcNow.Subtract(Date.Parse("01/01/1981")).TotalDays

        Dim best As Double = 0
        Dim bests As New List(Of Double)

        For Each p As AssetPrice In prices
            If p.diffFromMaxPrice > 80 Then Continue For
            ' reset
            If p.diffFromMaxPrice < 0.5 Then
                ' save it !
                If best >= 0.5 Then
                    If best > 40 Then
                        dbg.info(p.diffFromMaxPrice & "  " & p.ToString & "   " & best)
                    End If
                    bests.Add(best)
                End If

                best = 0
            ElseIf p.diffFromMaxPrice >= 0.5 Then
                If p.diffFromMaxPrice > best Then best = p.diffFromMaxPrice
            End If
        Next

        bests.Sort()

        Dim perPerc As Integer()
        perPerc = New Integer(55) {}

        For Each d As Double In bests
            If d > perPerc.Count - 1 Then
                'perPerc(perPerc.Count - 1) += 1
                For i = 0 To perPerc.Count - 1 Step 1
                    perPerc(i) += 1

                Next
            Else
                'perPerc(Math.Floor(d)) += 1
                For i = 0 To Math.Floor(d) Step 1
                    perPerc(i) += 1

                Next
            End If
        Next

        For perc As Integer = 0 To perPerc.Count - 1
            dbg.info("drop from max of " & perc & "% happen 1 ever " & Math.Round(10 * timespanDays / perPerc(perc) / 30) / 10 & " months (total " & perPerc(perc))
        Next


        ' we mesure how many time we got how much below max ever at the time. Since we also known the timespan, we thus have frequency.

        ' aggreg per percent bests






        Dim elapsed As Double = Date.UtcNow.Subtract(start).TotalMilliseconds
        writeText(New Point(img.Width - 50, 5), Math.Round(elapsed) & "ms", Color.Black, Color.Transparent, 12)
        PictureBoxAnalizeViewer.Image = img
    End Sub

    ' ------------------------------------------------------------------------------------------------------------------
    ' generate day/week/month  = f (% from max ever)

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim start As Date = Date.UtcNow
        Dim asset As AssetInfos = FrmMain.mainGraph.asset
        Dim assetHistory As AssetHistory = getAssetHistory(asset)

        paintItBlack()

        Dim prices As List(Of AssetPrice) = assetHistory.allPricesBetween(Date.Parse("01/01/1980"), Date.UtcNow)
        ' Dim dfi = DateTimeFormatInfo.CurrentInfo

        Dim perMonth As Integer()
        perMonth = New Integer(12) {}
        Dim perMonthStack As Double()
        perMonthStack = New Double(12) {}

        Dim perDayOfMonth As Integer()
        perDayOfMonth = New Integer(31) {}
        Dim perDayOfMonthStack As Double()
        perDayOfMonthStack = New Double(31) {}

        Dim perWeekOfYear As Integer()
        perWeekOfYear = New Integer(54) {}
        Dim perWeekOfYearStack As Double()
        perWeekOfYearStack = New Double(54) {}

        ' for each day of week
        ' for each week number of year

        Dim myCI As New CultureInfo("en-US")
        Dim myCal As System.Globalization.Calendar = myCI.Calendar
        Dim myCWR As CalendarWeekRule = myCI.DateTimeFormat.CalendarWeekRule
        Dim myFirstDOW As DayOfWeek = myCI.DateTimeFormat.FirstDayOfWeek


        For Each p As AssetPrice In prices
            perMonth(p.dat.Month) += 1
            perMonthStack(p.dat.Month) += p.diffFromMaxPrice
            perDayOfMonth(p.dat.Day) += 1
            perDayOfMonthStack(p.dat.Day) += p.diffFromMaxPrice

            Dim weekOfyear As Integer = myCal.GetWeekOfYear(p.dat, myCWR, myFirstDOW)

            perWeekOfYear(weekOfyear) += 1
            perWeekOfYearStack(weekOfyear) += p.diffFromMaxPrice
        Next

        ' per month
        ' mars/novembre is better

        For i = 0 To 11
            dbg.info("> month " & i & " average % from max ever " & perMonthStack(i) / perMonth(i))
        Next

        For i = 0 To 30
            dbg.info("> day of month " & i & " average % from max ever " & perDayOfMonthStack(i) / perDayOfMonth(i))
        Next

        For i = 0 To 51
            dbg.info("> day of week " & i & " average % from max ever " & perWeekOfYearStack(i) / perWeekOfYear(i))
        Next

        ' ... and we learn nothing !

        Dim elapsed As Double = Date.UtcNow.Subtract(start).TotalMilliseconds
        writeText(New Point(img.Width - 50, 5), Math.Round(elapsed) & "ms", Color.Black, Color.Transparent, 12)
        PictureBoxAnalizeViewer.Image = img
    End Sub


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
        Dim font As Font = New Font(DefaultFont.FontFamily, fontSize)
        g.DrawString(txt, font, brch, New Point(pt.X - 2, pt.Y - 4))
    End Sub
End Class