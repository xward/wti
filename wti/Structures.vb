﻿Module Structures


    'please take care  to sell trade with this strat



    ' ----------------------------------------------------------------------------------------



    Public Structure DegiroOrder
        Dim id As String
        Dim asset As AssetEnum
        Dim orderType As OrderTypeEnum
        ' all the values

        Dim referToPosition As String
    End Structure

    Public Structure DegiroPosition
        Dim id As String
        ' 3OIL
        Dim ticker As String
        ' IE00BMTM6B32
        Dim isin As String

        Dim quantity As Integer
        'how much is it now
        Dim totalValue As Double
        'how much you bought it
        Dim pru As Double

    End Structure

    ' the complete buy/sell tracking object
    '' currently bs filling
    Public Structure Trade
        ' id

        ' ME, X
        Dim owner As String


        ' buy ''''''''''''''''''''''''''''''''''''
        Dim buyDone As Boolean
        Dim buyOrderDegiroId As String
        Dim orderDate As Date


        ' STOP_BUY
        Dim buyStrat As String

        ' sell '''''''''''''''''''''''''''''''''''
        Dim sellDone As Boolean
        Dim sellOrderDegiroId As String
        Dim sellDate As Date

        Dim sellStrat As String

        ' at what value ?



        ' feedbacks ''''''''''''''''''''''''''''''
        ' Dim finalGain As Double

        'failure rate, count won more if stop buy

    End Structure


    Public Structure AssetPrice
        Dim ticker As String
        Dim price As Double
        Dim todayChangePerc As Double
        Dim dat As Date
    End Structure

    Public Structure AssetInfos
        ' if etf
        Dim ISIN As String
        Dim tradingViewUrl As String
        ' also called ticker
        Dim ticker As String
        ' if apply, only for raw like wti but not it's ETF version w or wo leverage
        Dim futurUrl As String
        Dim degiroOrderUrl As String
        Dim degireId As Integer
    End Structure


    Public Function assetInfo(asset As AssetEnum) As AssetInfos
        Select Case asset
            Case AssetEnum.WTI3x
                Return New AssetInfos With {
                    .ISIN = "IE00BMTM6B32",
                    .tradingViewUrl = "https://www.tradingview.com/chart/aSPkAHjR/?symbol=MIL%3A3OIL",
                    .ticker = "3OIL",
                    .degiroOrderUrl = "https://trader.degiro.nl/trader/?appMode=order#/markets?newOrder&action=buy&productId=18744180",
                    .degireId = 18744180
                }
            Case AssetEnum.WTI3xShort
                Return New AssetInfos With {
                    .ISIN = "XS2819844387",
                    .tradingViewUrl = "https://www.tradingview.com/chart/2OrM2Knv/?symbol=MIL%3A3OIS",
                    .ticker = "3OIS",
                    .degiroOrderUrl = "https://trader.degiro.nl/trader/?appMode=order#/markets?newOrder&action=buy&productId=30311482",
                    .degireId = 30311482
                }
        End Select
    End Function

    Public Enum AssetEnum
        WTI3x
        WTI3xShort
    End Enum

    Public Enum OrderTypeEnum
        LIMIT_SELL
        LIMIT_BUY
        ' stop loss
        STOP_SELL
        STOP_BUY
    End Enum
End Module
