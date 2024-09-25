Imports System.IO
Namespace SP500StrategyLab
    Module SP500Long
        ' THIS IS EPARGNE STUFF, NO TRADING (PEA, CTO EPARGNE, CTO LISE, CTO CHLOE)

        ' goal: savoir si sur mon pea quoi faire des x euro par mois que je mettrai, long terme stuff
        ' appliquable pour les CTO des filles, si vraiment il y a une diff
        ' DCA vs DVA/market_timing/j'attends la giga crise

        ' goal2: savoir si epargne long game 3x avec time market(crises) mieux que goal1



        'from data of https://www.nasdaq.com/market-activity/index/spx/historical  7 ans historique
        ' min max, average per day over 10 years

        '  il y a aussi https://finance.yahoo.com/quote/%5ESPX/history/?period1=-1325583000&period2=1725962871 avec daily depuis toujours avec volumes


        ' test following:
        ' PEA DCA classique 1/w et 1/m
        ' PEA DVA classique 1/w et 1/m

        ' au dca classique ajouter serenity

        ' DEGIRO DCA leverage 3x
        ' DEGIRO DVA leverage 3x
        ' serenity, tapis si chute 20% en x temps
        ' serenity, tapis sur 3x si chute 20% en x temps

        ' manage real world: fee, can't buy all cash even if I want, ester...
        ' ddos dva: changer period investisseent, perc to invest
        ' est ce que attendre une giga crise pour lump sum mieux que dca ?  

        ' do the same for msci world

        ' [ambush crises] ma préférée sans preuve so far: 
        ' - dca sur serenity
        ' - si sp500 baisse 15-20% (à check previous crises) par rapport max ever, on entre en crise
        ' - si crise: stop-buy-tapis 4% (à check previous crises) sur le 3x
        ' - quand ca a remonté 4.2x, stop-sell 3% below

        ' rerun for every day start all scenario, max 10year span
        ' at end with fiscalité, combien de % par an de gain au bout de 10 ans
        ' -------------------------------------------------------------------------------------------------------------------------------------------------

        Public Sub runAll()
            history = getAssetHistory(assetInfo(AssetNameEnum.SP500))

            serenityOnly()
            runDCA()
            'serenityLumpSumSurCrise()
            'runDCAWeek()
            'runDVAClassic()
            'runDVAClassicWeek()
        End Sub

        Public Sub serenityOnly()
            resetRun()
            setEarn(500, 30)
            epargne(500)
            useSerenityAtRest = True

            While nextTick()
            End While

            dbgLog("serenity only")
        End Sub

        Public Sub runDCA()
            resetRun(2014)
            setEarn(500, 30)
            epargne(500)
            buy()

            Dim max As Integer = 5000
            Dim i As Integer = 0
            While nextTick()
                'If i > max Then Exit While
                'i += 1
                If isPayDay() Then buy()
            End While

            ' ca fait un taux global de 8.7%, si peut acheter des fractions
            ' https://finary.com/fr/tools/compound-interests-calculator?initial_capital=500&monthly_savings=500&investment_horizon=55&interest_rate=8.7&interest_rate_every_x_months=12


            dbgLog("DCA classic")
        End Sub

        Public Sub serenityLumpSumSurCrise()
            resetRun()
            setEarn(500, 30)
            availableCash = 500
            useSerenityAtRest = True

            While nextTick()
                ' tapis !
                If auFondduTrou() Then
                    dbgLog()
                    buy()
                End If
            End While

            Debug.WriteLine(vbCrLf & "Serenity/lump sum crise only après: " & history.daySpanSize / 365 & " ans: ")
            dbgLog()
        End Sub



        Public Sub runDCAWeek()
            resetRun()
            setEarn(500.0 / 30 * 7, 7)
            availableCash = 100
            buy()

            While nextTick()
                If isPayDay() Then buy()
            End While

            Debug.WriteLine(vbCrLf & "DCA classic par semaine, après: " & history.daySpanSize / 365 & " ans: ")
            dbgLog()
        End Sub

        Public Sub runDVAClassic()
            resetRun()
            setEarn(500, 30)
            availableCash = 500
            ' useSerenityAtRest = True

            While nextTick()
                If isPayDay() Then
                    Dim thisMonthGain As Double = 1.1 ' history.currentPrice().close / history.ElementAt(day - 30).close

                    ' the repartition is very dumb, can be improved
                    ' =f(global trend), =f(max ever, now), do more linear
                    If thisMonthGain > 1.09 Then
                        ' buy(25)
                    ElseIf thisMonthGain < 0.95 Then
                        buy()
                        ' dbgLog()
                    Else
                        ' buy(50)
                    End If


                    'If thisMonthGain < 0.91 Then
                    '    portefeuilleValue += availableCash
                    '    availableCash = 0
                    'End If

                End If
            End While

            'i don't know why this one is 33%

            Debug.WriteLine(vbCrLf & "DVA classic par mois, après: " & history.daySpanSize / 365 & " ans: ")
            dbgLog()
        End Sub


        Public Sub runDVAClassicWeek()
            resetRun()
            setEarn(500.0 / 30 * 7, 7)
            availableCash = 100

            While nextTick()
                If isPayDay() Then
                    Dim thisWeekGain As Double = 1 ' history.currentPrice().close / history.ElementAt(day - 7).close

                    If thisWeekGain > 1.05 Then
                        ' buy(25)
                    ElseIf thisWeekGain < 0.95 Then
                        buy()
                    Else
                        ' buy(50)
                    End If

                End If
            End While

            Debug.WriteLine(vbCrLf & "DVA classic par semaine, après: " & history.daySpanSize / 365 & " ans: ")
            dbgLog()
        End Sub


        Private Sub dbgLog(Optional strategyName As String = "")


            Dim totalValue As Double = owned * history.currentPrice.price + availableCash


            Debug.WriteLine(vbCrLf & "running " & strategyName & " gives after " & Date.UtcNow.Year - startingYear & " years:")

            Debug.WriteLine("[" & history.replayProgressIndex & "/" & history.pricesCount & "] " & " [invested=" & Math.Round(totalInvested) &
                            "] -> [porteuille=" & Math.Round(owned * history.currentPrice.price) & " owned=" & Math.Round(100 * owned) / 100 & " cash=" & Math.Round(availableCash) & "]" &
                            " gain: " & Math.Round(10000 * (totalValue / totalInvested - 1)) / 100 & "% (+" & Math.Round(totalValue - totalInvested) & ")")
        End Sub

        ' -------------------------------------------------------------------------------------------------------------------------------------------------
        Private history As AssetHistory
        'you can have fraction of it
        Private owned As Double

        'how many days since we start epargning
        Private daysDone As Integer = 0

        Private daysSinceLastPayDay As Integer = 0
        Private previousDayWasPayDay As Boolean = False

        Private lastTickDayChange As Date

        Private iEarnEveryXDays As Integer
        Private earnAmount As Double

        Private availableCash As Double
        Private totalInvested As Double

        Private portefeuilleValue As Double
        Private useLeverage3 As Boolean
        Private useSerenityAtRest As Boolean

        Private brokerFee As Double
        Private brokerFeePerc As Double

        Private Sub buy(Optional percOfAvailableCashToUse As Integer = 100)
            Dim spending As Double = availableCash * percOfAvailableCashToUse / 100
            Dim price As Double = history.currentPrice.price

            owned += spending / price
            availableCash -= spending

            'dbg.info(spending & " " & price & "  " & owned & "  " & availableCash)
        End Sub

        Private aLongTimeAgo As Date = Date.Parse("01/01/1950")

        Private Function dayInt(d As Date) As Integer
            Return Math.Floor(d.Subtract(aLongTimeAgo).TotalDays)
        End Function

        ' 1900 ie forever before
        Private startingYear As Integer = 0
        Private Sub resetRun(Optional beginingYear As Integer = 1900)
            history.initReplay()

            history.replayNext()

            While history.currentPrice.dat.Year < beginingYear
                history.replayNext()
            End While

            startingYear = history.currentPrice.dat.Year


            lastTickDayChange = history.currentPrice.dat

            owned = 0

            daysDone = 0
            daysSinceLastPayDay = 0
            previousDayWasPayDay = False


            availableCash = 0
            totalInvested = 0
            portefeuilleValue = 0
            iEarnEveryXDays = 30

            earnAmount = 0
            useLeverage3 = False
            useSerenityAtRest = False
            brokerFee = 0
            brokerFeePerc = 0
        End Sub

        Public Sub setEarn(amount As Double, everyHowManyDays As Integer)
            earnAmount = amount
            iEarnEveryXDays = everyHowManyDays
        End Sub

        Public Function isPayDay() As Boolean
            Return daysSinceLastPayDay >= iEarnEveryXDays

            'Return False
            'Return Day Mod iEarnEveryXDays = 0
        End Function

        Private Sub setDegiro()
            brokerFee = 3
        End Sub

        Private Sub setBoursorama()
            brokerFeePerc = 0.6
        End Sub

        Private Sub epargne(amount As Double)
            availableCash += amount
            totalInvested += amount
        End Sub

        Private Function nextTick() As Boolean
            If Not history.replayNext() Then Return False

            Dim price As AssetPrice = history.currentPrice()

            ' dbg.info(price.ToString)

            Dim dayDiff As Integer = dayInt(price.dat) - dayInt(lastTickDayChange)

            If previousDayWasPayDay Then
                previousDayWasPayDay = False
                daysSinceLastPayDay = 0
            End If
            ' new day !
            If dayDiff > 0 Then
                lastTickDayChange = price.dat

                daysDone += dayDiff
                daysSinceLastPayDay += dayDiff

                If useSerenityAtRest Then availableCash += availableCash * dayDiff * 0.025 / 365.0

                If isPayDay() Then epargne(earnAmount)

                If isPayDay() Then previousDayWasPayDay = True
            End If





            Application.DoEvents()
            Return True
        End Function



        'If history.currentPrice().close = 0 Or history.currentPrice().open = 0 Then
        '    ' do nothing
        'Else
        '    If useLeverage3 Then
        '        If history.currentPrice().close / history.currentPrice().open > 1 Then
        '            portefeuilleValue = portefeuilleValue * (1 + 3 * (history.currentPrice().close / history.currentPrice().open - 1))
        '        Else
        '            portefeuilleValue = portefeuilleValue * (1 + 3.01 * (history.currentPrice().close / history.currentPrice().open - 1))
        '        End If
        '    Else
        '        portefeuilleValue = portefeuilleValue * history.currentPrice().close / history.currentPrice().open
        '    End If
        'End If


        Private Function auFondduTrou() As Double

            Dim dats As New List(Of String)

            ' covid
            dats.Add("03/20/2020")
            'creux 2022
            dats.Add("10/07/2022")

            For Each ds As String In dats
                Dim dat As Date = Date.Parse(ds)
                If history.currentPrice().dat.Year = dat.Year And history.currentPrice().dat.Month = dat.Month And history.currentPrice().dat.Day = dat.Day Then
                    ' dbg.info("HAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")
                    Return True
                End If
            Next
            Return False
        End Function

        'Public Sub loadSP500Historic()
        '    history.Clear()
        '    For Each l As String In File.ReadAllLines(CST.DATA_PATH & "/sp500Historical/SP500 HistoricalData_1725470811529.csv")
        '        If l.Contains("Date") Then Continue For

        '        '       Date,Close/Last,Open,High,   Low
        '        ' 09/03/2024,5528.93,5623.89,5623.89,5504.33

        '        Dim split As String() = l.Split(",")
        '        Dim day As New SP500Day With {
        '            .dat = Date.Parse(split.ElementAt(0)),
        '            .close = Double.Parse(split.ElementAt(1)),
        '            .open = Double.Parse(split.ElementAt(2)),
        '            .high = Double.Parse(split.ElementAt(3)),
        '            .low = Double.Parse(split.ElementAt(4))
        '        }
        '        history.Add(day)
        '    Next

        '    history.Reverse()
        'End Sub



        'Private history As New List(Of SP500Day)
        'Private Structure SP500Day
        '    Dim dat As Date
        '    Dim open As Double
        '    Dim close As Double
        '    Dim high As Double
        '    Dim low As Double
        'End Structure
    End Module

End Namespace

