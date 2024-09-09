Imports System.IO
Namespace SP500Long
    Module SP500Long
        'from data of https://www.nasdaq.com/market-activity/index/spx/historical
        ' min max, average per day over 10 years

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

        ' rerun for every day start all scenario, max 10year span
        ' at end with fiscalité, combien de % par an de gain au bout de 10 ans
        ' -------------------------------------------------------------------------------------------------------------------------------------------------




        Public Sub runAll()
            loadSP500Historic()
            serenityOnly()
            serenityLumpSumSurCrise()
            runDCA()
            runDCAWeek()
            runDVAClassic()
            runDVAClassicWeek()
        End Sub

        Public Sub serenityOnly()
            resetRun()
            setEarn(500, 30)
            availableCash = 500
            useSerenityAtRest = True

            While nextDay()
            End While

            Debug.WriteLine(vbCrLf & "Serenity only après: " & history.Count / 365 & " ans: ")
            dbgLog()
        End Sub

        Public Sub serenityLumpSumSurCrise()
            resetRun()
            setEarn(500, 30)
            availableCash = 500
            useSerenityAtRest = True

            While nextDay()
                ' tapis !
                If auFondduTrou() Then buy()
            End While

            Debug.WriteLine(vbCrLf & "Serenity/lump sum crise only après: " & history.Count / 365 & " ans: ")
            dbgLog()
        End Sub

        Public Sub runDCA()
            resetRun()
            setEarn(500, 30)
            availableCash = 500
            buy()

            While nextDay()
                If isPAyDay() Then buy()
            End While

            Debug.WriteLine(vbCrLf & "DCA classic par mois, après: " & history.Count / 365 & " ans: ")
            dbgLog()
        End Sub

        Public Sub runDCAWeek()
            resetRun()
            setEarn(500.0 / 30 * 7, 7)
            availableCash = 100
            buy()

            While nextDay()
                If isPAyDay() Then buy()
            End While

            Debug.WriteLine(vbCrLf & "DCA classic par semaine, après: " & history.Count / 365 & " ans: ")
            dbgLog()
        End Sub

        Public Sub runDVAClassic()
            resetRun()
            setEarn(500, 30)
            availableCash = 500
            'useSerenityAtRest = True

            While nextDay()
                If isPAyDay() Then
                    Dim thisMonthGain As Double = todaySp500.close / history.ElementAt(day - 30).close

                    ' the repartition is very dumb, can be improved
                    ' =f(global trend), =f(max ever, now), do more linear
                    If thisMonthGain > 1.09 Then
                        ' buy(25)
                    ElseIf thisMonthGain < 0.95 Then
                        buy()
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

            Debug.WriteLine(vbCrLf & "DVA classic par mois, après: " & history.Count / 365 & " ans: ")
            dbgLog()
        End Sub


        Public Sub runDVAClassicWeek()
            resetRun()
            setEarn(500.0 / 30 * 7, 7)
            availableCash = 100

            While nextDay()
                If isPAyDay() Then
                    Dim thisWeekGain As Double = todaySp500.close / history.ElementAt(day - 7).close

                    If thisWeekGain > 1.05 Then
                        ' buy(25)
                    ElseIf thisWeekGain < 0.95 Then
                        buy()
                    Else
                        ' buy(50)
                    End If

                End If
            End While

            Debug.WriteLine(vbCrLf & "DVA classic par semaine, après: " & history.Count / 365 & " ans: ")
            dbgLog()
        End Sub


        Private Sub dbgLog()
            Debug.WriteLine("[" & day & "/" & history.Count & "] " & todaySp500.dat & " invested=" & (Math.Round(100 * totalInvested) / 100).ToString("######.##") &
                            " porteuille=" & (Math.Round(100 * portefeuilleValue) / 100).ToString("######.##") & " cash=" & availableCash & " plus value=" & (Math.Round(100 * (portefeuilleValue + availableCash - totalInvested)) / 100).ToString("######.##") &
                            " gain: " & Math.Round(10000 * ((portefeuilleValue + availableCash) / (totalInvested) - 1)) / 100 & "%")
        End Sub

        ' -------------------------------------------------------------------------------------------------------------------------------------------------
        Private day As Integer
        Private availableCash As Double
        Private totalInvested As Double
        Private portefeuilleValue As Double
        Private todaySp500 As SP500Day
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
            day = 0
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
            Return day Mod iEarnEveryXDays = 0
        End Function

        Private Sub setDegiro()
            brokerFee = 3
        End Sub

        Private Sub setBoursorama()
            brokerFeePerc = 0.6
        End Sub

        Private Function nextDay() As Boolean
            If day = history.Count Then Return False
            todaySp500 = history.ElementAt(day)

            If todaySp500.close = 0 Or todaySp500.open = 0 Then
                ' do nothing
            Else
                If useLeverage3 Then
                    If todaySp500.close / todaySp500.open > 1 Then
                        portefeuilleValue = portefeuilleValue * (1 + 3 * (todaySp500.close / todaySp500.open - 1))
                    Else
                        portefeuilleValue = portefeuilleValue * (1 + 3.01 * (todaySp500.close / todaySp500.open - 1))
                    End If
                Else
                    portefeuilleValue = portefeuilleValue * todaySp500.close / todaySp500.open
                End If
            End If

            Dim newCash As Double = 0
            If useSerenityAtRest Then newCash += availableCash * (0.02 / 365)

            If isPAyDay() Then newCash += earnAmount

            availableCash += newCash
            totalInvested += newCash
            day += 1

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
                If todaySp500.dat.Year = dat.Year And todaySp500.dat.Month = dat.Month And todaySp500.dat.Day = dat.Day Then
                    ' dbg.info("HAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")
                    Return True
                End If
            Next
            Return False
        End Function

        Public Sub loadSP500Historic()
            history.Clear()
            For Each l As String In File.ReadAllLines(CST.DATA_PATH & "/sp500Historical/SP500 HistoricalData_1725470811529.csv")
                If l.Contains("Date") Then Continue For

                '       Date,Close/Last,Open,High,   Low
                ' 09/03/2024,5528.93,5623.89,5623.89,5504.33

                Dim split As String() = l.Split(",")
                Dim day As New SP500Day With {
                    .dat = Date.Parse(split.ElementAt(0)),
                    .close = Double.Parse(split.ElementAt(1)),
                    .open = Double.Parse(split.ElementAt(2)),
                    .high = Double.Parse(split.ElementAt(3)),
                    .low = Double.Parse(split.ElementAt(4))
                }
                history.Add(day)
            Next

            history.Reverse()
        End Sub



        Private history As New List(Of SP500Day)
        Private Structure SP500Day
            Dim dat As Date
            Dim open As Double
            Dim close As Double
            Dim high As Double
            Dim low As Double
        End Structure
    End Module

End Namespace

