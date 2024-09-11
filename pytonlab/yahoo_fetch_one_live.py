import yfinance as yf
import os
import time
from pathlib import Path
import datetime
from datetime import date
import sys

print ('argument list', sys.argv)

TICKER=sys.argv[1]

today = date.today()
d = today
next_d = d + datetime.timedelta(days=1)

data = yf.download(TICKER, start=d.strftime("%Y-%m-%d"), end=next_d.strftime("%Y-%m-%d"), period="1d", interval="1m")
# print(data)
print(data['Close'].iloc[-1])
