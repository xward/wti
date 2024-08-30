Module Structures
    Public Structure Order
        Dim id As String
        Dim asset As AssetEnum
        Dim orderType As OrderTypeEnum
        ' all the values

        Dim referToPosition As String
    End Structure

    Public Structure Position
        Dim id As String
        Dim asset As AssetEnum
        ' can be negative
        Dim gain As Double

        ' previous gain
    End Structure


    Public Structure AssetPrice
        Dim ticker As String
        Dim price As Double
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
