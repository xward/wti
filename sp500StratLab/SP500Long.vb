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
            'serenityLumpSumSurCrise()
            'runDCA()
            'runDCAWeek()
            'runDVAClassic()
            'runDVAClassicWeek()
        End Sub

        Public Sub serenityOnly()
            resetRun()
            setEarn(500, 30)
            'push now
            epargne(500)
            useSerenityAtRest = True

            While nextTick()
            End While

            Debug.WriteLine(vbCrLf & "Serenity only après: " & history.daySpanSize / 365 & " ans: ")
            dbgLog()
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

        Public Sub runDCA()
            resetRun()
            setEarn(500, 30)
            availableCash = 500
            buy()

            While nextTick()
                If isPAyDay() Then buy()
            End While

            Debug.WriteLine(vbCrLf & "DCA classic par mois, après: " & history.daySpanSize / 365 & " ans: ")
            dbgLog()
        End Sub

        Public Sub runDCAWeek()
            resetRun()
            setEarn(500.0 / 30 * 7, 7)
            availableCash = 100
            buy()

            While nextTick()
                If isPAyDay() Then buy()
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
                If isPAyDay() Then
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
                If isPAyDay() Then
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


        Private Sub dbgLog()
            Debug.WriteLine("[" & history.replayProgressIndex & "/" & history.pricesCount & "] " & " [invested=" & Math.Round(totalInvested) &
                            "] -> [porteuille=" & Math.Round(portefeuilleValue) & " cash=" & Math.Round(availableCash) & "]" &
                            " gain: " & Math.Round(10000 * ((portefeuilleValue + availableCash) / totalInvested - 1)) / 100 & "% (+" & Math.Round(portefeuilleValue + availableCash - totalInvested) & ")")
        End Sub

        ' -------------------------------------------------------------------------------------------------------------------------------------------------
        Private history As AssetHistory
        'you can have fraction of it
        Private owned As Double

        Private currentTickIsPayDay As Boolean


        Private availableCash As Double
        Private totalInvested As Double
        Private portefeuilleValue As Double
        Private iEarnEveryXDays As Integer
        Private earnAmount As Double
        Private useLeverage3 As Boolean
        Private useSerenityAtRest As Boolean

        Private brokerFee As Double
        Private brokerFeePerc As Double

        Private Sub buy(Optional percOfAvailableCashToUse As Integer = 100)
            portefeuilleValue += availableCash * percOfAvailableCashToUse / 100
            availableCash *= 1 - percOfAvailableCashToUse / 100
        End Sub

        Private Sub resetRun()
            history.initReplay()

            owned = 0

            currentTickIsPayDay = False


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

        Public Function isPAyDay() As Boolean
            Return False
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

            ' history.currentPrice()


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

            Dim newCash As Double = 0
            'fix: do it once per day ... not 4 !
            If useSerenityAtRest Then availableCash += availableCash * (0.02 / 365)

            If isPAyDay() Then epargne(earnAmount)


            Application.DoEvents()
            Return True
        End Function

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

