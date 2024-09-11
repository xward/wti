# https://github.com/ranaroussi/yfinance?tab=readme-ov-file#quick-start
# https://aroussi.com/post/python-yahoo-finance

import yfinance as yf

msft = yf.Ticker("MSFT")

# get all stock info
msft.info

print(msft.info)

exit()


{'address1': 'One Microsoft Way', 'city': 'Redmond', 'state': 'WA', 'zip': '98052-6399', 'country': 'United States', 'phone': '425 882 8080', 'website': 'https://www.microsoft.com', 
'industry': 'Software - Infrastructure', 'industryKey': 'software-infrastructure', 'industryDisp': 'Software - Infrastructure', 'sector': 'Technology', 'sectorKey': 'technology', 'sectorDisp': 'Technology', 
'longBusinessSummary': 'Microsoft Corporation develops and supports software, services, devices and solutions worldwide. The Productivity and Business Processes segment offers office, exchange, SharePoint, Microsoft Teams, office 365 Security and Compliance, Microsoft viva, and Microsoft 365 copilot; and office consumer services, such as Microsoft 365 consumer subscriptions, Office licensed on-premises, and other office services. This segment also provides LinkedIn; and dynamics business solutions, including Dynamics 365, a set of intelligent, cloud-based applications across ERP, CRM, power apps, and power automate; and on-premises ERP and CRM applications. The Intelligent Cloud segment offers server products and cloud services, such as azure and other cloud services; SQL and windows server, visual studio, system center, and related client access licenses, as well as nuance and GitHub; and enterprise services including enterprise support services, industry solutions, and nuance professional services. The More Personal Computing segment offers Windows, including windows OEM licensing and other non-volume licensing of the Windows operating system; Windows commercial comprising volume licensing of the Windows operating system, windows cloud services, and other Windows commercial offerings; patent licensing; and windows Internet of Things; and devices, such as surface, HoloLens, and PC accessories. Additionally, this segment provides gaming, which includes Xbox hardware and content, and first- and third-party content; Xbox game pass and other subscriptions, cloud gaming, advertising, third-party disc royalties, and other cloud services; and search and news advertising, which includes Bing, Microsoft News and Edge, and third-party affiliates. The company sells its products through OEMs, distributors, and resellers; and directly through digital marketplaces, online, and retail stores. The company was founded in 1975 and is headquartered in Redmond, Washington.', 
'fullTimeEmployees': 228000, 'companyOfficers': [{'maxAge': 1, 'name': 'Mr. Satya  Nadella', 'age': 56, 'title': 'Chairman & CEO', 'yearBorn': 1967, 'fiscalYear': 2023, 'totalPay': 9276400, 'exercisedValue': 0, 'unexercisedValue': 0},
{'maxAge': 1, 'name': 'Mr. Bradford L. Smith LCA', 'age': 64, 'title': 'President & Vice Chairman', 'yearBorn': 1959, 'fiscalYear': 2023, 'totalPay': 3591277, 'exercisedValue': 0, 'unexercisedValue': 0}, 
{'maxAge': 1, 'name': 'Ms. Amy E. Hood', 'age': 51, 'title': 'Executive VP & CFO', 'yearBorn': 1972, 'fiscalYear': 2023, 'totalPay': 3452196, 'exercisedValue': 0, 'unexercisedValue': 0}, 
{'maxAge': 1, 'name': 'Mr. Judson B. Althoff', 'age': 50, 'title': 'Executive VP & Chief Commercial Officer', 'yearBorn': 1973, 'fiscalYear': 2023, 'totalPay': 3355797, 'exercisedValue': 0, 'unexercisedValue': 0}, 
{'maxAge': 1, 'name': 'Mr. Christopher David Young', 'age': 51, 'title': 'Executive Vice President of Business Development, Strategy & Ventures', 'yearBorn': 1972, 'fiscalYear': 2023, 'totalPay': 2460507, 'exercisedValue': 0, 'unexercisedValue': 0}, 
{'maxAge': 1, 'name': 'Ms. Alice L. Jolla', 'age': 57, 'title': 'Corporate VP & Chief Accounting Officer', 'yearBorn': 1966, 'fiscalYear': 2023, 'exercisedValue': 0, 'unexercisedValue': 0}, 
{'maxAge': 1, 'name': 'Mr. James Kevin Scott', 'age': 51, 'title': 'Executive VP of AI & CTO', 'yearBorn': 1972, 'fiscalYear': 2023, 'exercisedValue': 0, 'unexercisedValue': 0}, {'maxAge': 1, 'name': 'Brett  Iversen', 'title': 'Vice President of Investor Relations', 'fiscalYear': 2023, 'exercisedValue': 0, 'unexercisedValue': 0}, {'maxAge': 1, 'name': 'Mr. Hossein  Nowbar', 'title': 'Chief Legal Officer', 'fiscalYear': 2023, 'exercisedValue': 0, 'unexercisedValue': 0}, {'maxAge': 1, 'name': 'Mr. Frank X. Shaw', 'title': 'Chief Communications Officer', 'fiscalYear': 2023, 'exercisedValue': 0, 'unexercisedValue': 0}], 
'auditRisk': 3, 'boardRisk': 4, 'compensationRisk': 2, 'shareHolderRightsRisk': 2, 'overallRisk': 1, 'governanceEpochDate': 1725148800, 'compensationAsOfEpochDate': 1703980800, 'irWebsite': 'http://www.microsoft.com/investor/default.aspx', 'maxAge': 86400, 'priceHint': 2,
'previousClose': 405.72, 'open': 408.18, 'dayLow': 407.7, 'dayHigh': 416.33, 'regularMarketPreviousClose': 405.72, 'regularMarketOpen': 408.18, 'regularMarketDayLow': 407.7, 'regularMarketDayHigh': 416.33, 
'dividendRate': 3.0, 'dividendYield': 0.0074, 'exDividendDate': 1723680000, 'payoutRatio': 0.2483, 'fiveYearAvgDividendYield': 0.9, 'beta': 0.896, 'trailingPE': 35.07197, 'forwardPE': 30.167519, 'volume': 19481468, 'regularMarketVolume': 19481468, 'averageVolume': 19805819, 'averageVolume10days': 16741170, 'averageDailyVolume10Day': 16741170, 'bid': 387.36, 'ask': 436.79, 'bidSize': 100, 'askSize': 100, 'marketCap': 3078765150208, 'fiftyTwoWeekLow': 309.45, 'fiftyTwoWeekHigh': 468.35, 'priceToSalesTrailing12Months': 12.560134, 'fiftyDayAverage': 427.0848, 'twoHundredDayAverage': 411.5326, 'trailingAnnualDividendRate': 3.0, 'trailingAnnualDividendYield': 0.007394262, 'currency': 'USD', 'enterpriseValue': 3038053400576, 'profitMargins': 0.35956, 'floatShares': 7422706458, 'sharesOutstanding': 7433039872, 'sharesShort': 54789102, 'sharesShortPriorMonth': 61931266, 'sharesShortPreviousMonthDate': 1721001600, 'dateShortInterest': 1723680000, 'sharesPercentSharesOut': 0.0074, 'heldPercentInsiders': 0.00054000004, 'heldPercentInstitutions': 0.73737997, 'shortRatio': 2.37, 'shortPercentOfFloat': 0.0074, 'impliedSharesOutstanding': 7433039872, 'bookValue': 36.115, 'priceToBook': 11.468919, 'lastFiscalYearEnd': 1719705600, 'nextFiscalYearEnd': 1751241600, 'mostRecentQuarter': 1719705600, 'earningsQuarterlyGrowth': 0.097, 'netIncomeToCommon': 88135999488, 'trailingEps': 11.81, 'forwardEps': 13.73, 'pegRatio': 2.61, 'lastSplitFactor': '2:1', 'lastSplitDate': 1045526400, 'enterpriseToRevenue': 12.394, 'enterpriseToEbitda': 23.472, '52WeekChange': 0.22289538, 'SandP52WeekChange': 0.22617042, 'lastDividendValue': 0.75, 'lastDividendDate': 1723680000, 'exchange': 'NMS', 'quoteType': 'EQUITY', 'symbol': 'MSFT', 'underlyingSymbol': 'MSFT', 'shortName': 'Microsoft Corporation', 'longName': 'Microsoft Corporation', 'firstTradeDateEpochUtc': 511108200, 'timeZoneFullName': 'America/New_York', 'timeZoneShortName': 'EDT', 'uuid': 'b004b3ec-de24-385e-b2c1-923f10d3fb62', 'messageBoardId': 'finmb_21835', 'gmtOffSetMilliseconds': -14400000, 'currentPrice': 414.2, 'targetHighPrice': 541.16, 'targetLowPrice': 396.85, 'targetMeanPrice': 447.97, 'targetMedianPrice': 450.97, 'recommendationMean': 1.7, 'recommendationKey': 'buy', 'numberOfAnalystOpinions': 47, 'totalCash': 75531001856, 'totalCashPerShare': 10.162, 'ebitda': 129433001984, 'totalDebt': 97851998208, 'quickRatio': 1.141, 'currentRatio': 1.275, 'totalRevenue': 245122007040, 'debtToEquity': 36.447, 'revenuePerShare': 32.986, 'returnOnAssets': 0.14802, 'returnOnEquity': 0.37133, 'freeCashflow': 56705249280, 'operatingCashflow': 118547996672, 'earningsGrowth': 0.097, 'revenueGrowth': 0.152, 'grossMargins': 0.69764, 'ebitdaMargins': 0.52804, 'operatingMargins': 0.43143, 'financialCurrency': 'USD', 'trailingPegRatio': 2.1881}


# get historical market data
hist = msft.history(period="1mo")

# show meta information about the history (requires history() to be called first)
msft.history_metadata

# show actions (dividends, splits, capital gains)
msft.actions
msft.dividends
msft.splits
msft.capital_gains  # only for mutual funds & etfs

# show share count
msft.get_shares_full(start="2022-01-01", end=None)

# show financials:
msft.calendar
msft.sec_filings
# - income statement
msft.income_stmt
msft.quarterly_income_stmt
# - balance sheet
msft.balance_sheet
msft.quarterly_balance_sheet
# - cash flow statement
msft.cashflow
msft.quarterly_cashflow
# see `Ticker.get_income_stmt()` for more options

# show holders
msft.major_holders
msft.institutional_holders
msft.mutualfund_holders
msft.insider_transactions
msft.insider_purchases
msft.insider_roster_holders

msft.sustainability

# show recommendations
msft.recommendations
msft.recommendations_summary
msft.upgrades_downgrades

# show analysts data
msft.analyst_price_targets
msft.earnings_estimate
msft.revenue_estimate
msft.earnings_history
msft.eps_trend
msft.eps_revisions
msft.growth_estimates

# Show future and historic earnings dates, returns at most next 4 quarters and last 8 quarters by default.
# Note: If more are needed use msft.get_earnings_dates(limit=XX) with increased limit argument.
msft.earnings_dates

# show ISIN code - *experimental*
# ISIN = International Securities Identification Number
msft.isin

# show options expirations
msft.options

# show news
msft.news

# get option chain for specific expiration
opt = msft.option_chain('YYYY-MM-DD')
# data available via: opt.calls, opt.puts