Public Module AssetInfom
    Public Structure AssetInfos
        ' if etf
        Dim ISIN As String
        'surname to disaplay
        Dim name As AssetNameEnum
        Dim fullName As String
        Dim ticker As String
        Dim yahooTicker As String
        Dim leverage As Integer
        Dim isShort As Boolean
        Dim currency As String
        Dim marketOpen As Date
        Dim marketUTCClose As Date

        ' data sources
        Dim updateDateFromSource As Boolean
        Dim updateSource As UpdateSourceEnum
        Dim updatePeriodSec As Integer
        Dim tradingViewUrl As String
        ' useless
        Dim yahooUrl As String

        Dim persistHistoryFromLiveCollect As Boolean
        Dim populateDataFromYahooMinute As Boolean
        Dim populateDataFromYahooDaily As Boolean

        ' graph configuration
        Dim lineColor As Color

        ' degiro
        Dim degiroOrderUrl As String
        Dim degireId As Integer

    End Structure

    Public Function assetInfo(assetTicker As String) As AssetInfos
        Return assetInfo(assetNameFromTicker(assetTicker))
    End Function

    Public Function assetInfo(assetName As AssetNameEnum) As AssetInfos
        Dim asset As New AssetInfos
        asset.ticker = " nothing"

        Select Case assetName
            Case AssetNameEnum.WTI
                With asset
                    .ticker = "CL=F"
                    .yahooTicker = "CL=F"
                    .name = AssetNameEnum.WTI
                    .fullName = "Crude Oil WTI"
                    .leverage = 1
                    .isShort = False
                    .currency = "$"
                    ' data source
                    .updateDateFromSource = True
                    .updateSource = UpdateSourceEnum.YAHOO
                    .updatePeriodSec = 45
                    .persistHistoryFromLiveCollect = False
                End With
            Case AssetNameEnum.WTI_3X
                With asset
                    .ticker = "3OIL"
                    .ISIN = "IE00BMTM6B32"
                    .name = AssetNameEnum.WTI_3X
                    .fullName = "WISDOMTREE WTI CRUDE OIL 3X DAILY L"
                    .leverage = 3
                    .isShort = False
                    .currency = "€"
                    ' data source
                    .updateDateFromSource = False
                    .updateSource = UpdateSourceEnum.TRADING_VIEW
                    .tradingViewUrl = "https://www.tradingview.com/chart/aSPkAHjR/?symbol=MIL%3A3OIL"
                    .updatePeriodSec = 5
                    .persistHistoryFromLiveCollect = CST.HOST_NAME = CST.CST.hostNameEnum.GHOST
                    ' degiro
                    .degiroOrderUrl = "https://trader.degiro.nl/trader/?appMode=order#/markets?newOrder&action=buy&productId=18744180"
                    .degireId = 18744180
                End With


            Case AssetNameEnum.WTI_3X_SHORT
                Dim tradingViewUrl As String = ""
                Select Case CST.HOST_NAME
                    Case CST.CST.hostNameEnum.GALACTICA
                        tradingViewUrl = "https://www.tradingview.com/chart/2OrM2Knv/?symbol=MIL%3A3OIS"
                    Case CST.CST.hostNameEnum.GHOST
                        tradingViewUrl = "https://www.tradingview.com/chart/vjhxMR0Z/?symbol=MIL%3A3OIS"
                End Select

                With asset
                    .ticker = "3OIS"
                    .ISIN = "XS2819844387"
                    .name = AssetNameEnum.WTI_3X_SHORT
                    .fullName = "WISDOMTREE WTI CRUDE OIL 3X DAILY SHO"
                    .leverage = 3
                    .isShort = True
                    .currency = "€"
                    ' data source
                    .updateDateFromSource = False
                    .updateSource = UpdateSourceEnum.TRADING_VIEW
                    .tradingViewUrl = tradingViewUrl
                    .updatePeriodSec = 5
                    .persistHistoryFromLiveCollect = CST.HOST_NAME = CST.CST.hostNameEnum.GHOST
                    ' degiro
                    .degiroOrderUrl = "https://trader.degiro.nl/trader/?appMode=order#/markets?newOrder&action=buy&productId=30311482"
                    .degireId = 30311482
                End With

            Case AssetNameEnum.SP500
                'blackrock sp500 euro IE00B3ZW0K18  iShares S&P 500 EUR Hedged UCITS ETF (Acc) (IUSE.L)
                ' IE000D3BWBR2 dollar iShares S&P 500 Swap UCITS ETF USD (Dist) https://finance.yahoo.com/quote/I50D.AS/
                ' LU1135865084 euro Amundi S&P 500 II UCITS ETF Acc (SP5C.PA)  https://finance.yahoo.com/quote/SP5C.PA/

                With asset
                    .name = AssetNameEnum.SP500
                    .fullName = "S&P 500 INDEX (^SPX)"
                    .ticker = "SPX"
                    .yahooTicker = "^SPX"
                    .leverage = 1
                    .isShort = False
                    .currency = "$"
                    .degiroOrderUrl = "-"
                    .degireId = 0
                    .lineColor = Color.DarkBlue
                    ' data source
                    .updateDateFromSource = False
                    .updateSource = UpdateSourceEnum.YAHOO
                    .updatePeriodSec = 45
                    .yahooUrl = "https://finance.yahoo.com/quote/%5ESPX/"
                    .persistHistoryFromLiveCollect = False
                    ' heavy !! also will cause bug on dynamic currentMaxPrice on data load if running with populateDataFromYahooDaily
                    .populateDataFromYahooMinute = False
                    .populateDataFromYahooDaily = True
                    ' degiro
                    ' nothing
                End With
                        ' close  4:51 PM EDT
                        ' open 13h30 utc
                        'but data from yahoo: 09h30 to 16h

            Case AssetNameEnum.SP500_3X
                Dim tradingViewUrl As String = ""
                Select Case CST.HOST_NAME
                    Case CST.CST.hostNameEnum.GALACTICA
                        tradingViewUrl = "https://www.tradingview.com/chart/2OrM2Knv/?symbol=MIL%3A3USL"
                    Case CST.CST.hostNameEnum.GHOST
                        tradingViewUrl = "https://www.tradingview.com/chart/vjhxMR0Z/?symbol=MIL%3A3USL"
                End Select

                ' https://finance.yahoo.com/quote/IE00B7Y34M31.SG/

                With asset
                    .ISIN = "IE00B7Y34M31"
                    .name = AssetNameEnum.SP500_3X
                    .fullName = "WISDOMTREE S&P 500 3X DAILY LEVERAG"
                    .yahooUrl = "https://finance.yahoo.com/quote/3USL.L/"
                    .ticker = "3USL"
                    .yahooTicker = "3USL.L"
                    .leverage = 3
                    .isShort = False
                    .currency = "€"
                    .marketOpen = Date.Parse("01/01/2024 07:00")
                    .marketUTCClose = Date.Parse("01/01/2024 15:45")
                    .lineColor = Color.Blue
                    ' data source
                    .updateDateFromSource = CST.HOST_NAME = whoIsCollecting ' CST.HOST_NAME = CST.CST.hostNameEnum.GHOST
                    .updateSource = UpdateSourceEnum.TRADING_VIEW
                    .updatePeriodSec = 5
                    .tradingViewUrl = tradingViewUrl
                    .persistHistoryFromLiveCollect = CST.HOST_NAME = whoIsCollecting ' CST.HOST_NAME = CST.CST.hostNameEnum.GHOST
                    ' degiro
                    .degiroOrderUrl = "https://trader.degiro.nl/trader/?appMode=order#/markets?newOrder&action=buy&productId=4995112"
                    .degireId = 4995112
                End With



            Case AssetNameEnum.PEA_SP500
                With asset
                    .name = AssetNameEnum.PEA_SP500
                    .fullName = "Lyxor PEA S&P 500 UCITS ETF - Capi."
                    .ticker = "1rTPSP5"
                    .yahooTicker = "PSP5.PA"
                    .leverage = 1
                    .isShort = False
                    .currency = "€"
                    .lineColor = Color.DarkGreen
                    ' data source
                    .updateDateFromSource = False
                    .updateSource = UpdateSourceEnum.YAHOO
                    .updatePeriodSec = 45
                    .persistHistoryFromLiveCollect = False
                    .populateDataFromYahooMinute = True
                    .populateDataFromYahooDaily = True
                    .yahooUrl = "https://www.boursorama.com/bourse/trackers/cours/1rTPSP5/"

                    ' degiro
                    .degiroOrderUrl = "-"
                    .degireId = 0
                End With

        End Select
        Return asset
    End Function

    Public Function assetNameFromTicker(ticker As String) As AssetNameEnum
        Select Case ticker
            Case "SPX"
                Return AssetNameEnum.SP500
            Case "1rTPSP5"
                Return AssetNameEnum.PEA_SP500
            Case "3USL"
                Return AssetNameEnum.SP500_3X
            Case "CL=F"
                Return AssetNameEnum.WTI
            Case "3OIL"
                Return AssetNameEnum.WTI_3X
            Case "3OIS"
                Return AssetNameEnum.WTI_3X_SHORT
        End Select
        Return Nothing
    End Function


    Public Function assetFromName(name As AssetNameEnum) As AssetInfos
        Select Case name
            Case AssetNameEnum.SP500
                Return assetInfo("SPX")
            Case AssetNameEnum.PEA_SP500
                Return assetInfo("1rTPSP5")
            Case AssetNameEnum.SP500_3X
                Return assetInfo("3USL")
            Case AssetNameEnum.WTI
                Return assetInfo("CL=F")
            Case AssetNameEnum.WTI_3X
                Return assetInfo("3OIL")
            Case AssetNameEnum.WTI_3X_SHORT
                Return assetInfo("3OIS")
        End Select
        Return Nothing
    End Function

    Public Enum AssetNameEnum
        UNKOWN
        ' dollar us date
        SP500
        ' europe version for live trading on 3x
        PEA_SP500
        SP500_3X
        WTI
        WTI_3X
        WTI_3X_SHORT
    End Enum

    Public Enum UpdateSourceEnum
        NONE
        TRADING_VIEW
        YAHOO
        BOURSOBANK
        DEGIRO
    End Enum
End Module
