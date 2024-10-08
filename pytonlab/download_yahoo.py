# launch me once a day, all days

import yfinance as yf
import os
import time
from pathlib import Path
import datetime
from datetime import date

PATH="../data/yahoo"

SP500="^SPX"
# amundi Lyxor PEA S&P 500 UCITS ETF - Capi. FR0011871128
PEA_SP500="PSP5.PA"
# sp500 3x usd IE00B7Y34M31
SP5003x="3USL.L"
# PEA+CT AMUNDI ETF MSCI WR LU1681043599
MSCI="CW8.PA"

# wti ? copper ? platinium ? nasdaq ? gold ? crypto ?
# msci ? cac40 ?

perMinute = [SP5003x, SP500, PEA_SP500, MSCI]
perDay = [SP5003x, SP500, PEA_SP500, MSCI]

today = date.today()

def file_exist(name):
  return Path(f'{PATH}/{name}').is_file()

# download per minute, one file per day
for t in perMinute:
  # check last 2 days if exist
  for past_day in range(1, 3):
    d = today + datetime.timedelta(days=-past_day)
    next_d = d + datetime.timedelta(days=1)

    file_name = f'{t}_{d.strftime("%Y_%m_%d")}.minute.yahoo.txt'
    print(f'check {file_name}')
    if file_exist(file_name): continue
    print(f'Downloading {file_name} ...')
    data = yf.download(t, start=d.strftime("%Y-%m-%d"), end=next_d.strftime("%Y-%m-%d"), period="1d", interval="1m")
    data.to_csv(f'{PATH}/{file_name}', sep=";")
    time.sleep(1)

# download/update year-long data per day

for t in perDay:
  file_name = f'{t}.daily.yahoo.txt'
  print(f'check {file_name}')
  if not file_exist(file_name) or date.today() != datetime.date.fromtimestamp(os.path.getctime(f'{PATH}/{file_name}')):
    yf.download(t, start="1970-01-01", end="2025-01-01", period="1d", interval="1d").to_csv(f'{PATH}/{file_name}', sep=";")
    time.sleep(1)
