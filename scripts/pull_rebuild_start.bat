:: kill

taskkill /F /IM WorstTradingInitiative.exe

:: git pull, do we git stash ?
"C:\Program Files\Git\bin\sh.exe" --login -i -c "git pull"

:: download data from yahoo
py ../pytonlab/download_yahoo.py > yahoo_fetch.log


CD ..

:: push any spp5003x data from ghost
"C:\Program Files\Git\bin\sh.exe" --login -i -c "git add data/dataFromThePast/3USL_*;git commit -m update_3USL_data; git push origin master"
:: push all data from yahoo folder data
"C:\Program Files\Git\bin\sh.exe" --login -i -c "git add data/yahoo/*;git commit -m update_yahoo_data; git push origin master"

:: build
SET SLN=WorstTradingInitiative.sln
SET MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe
"%MSBUILD%" "%SLN%" /p:Configuration=Debug

:: we change path to have WorstTradingInitiative.exe output everything in bin\Debug\net8.0-windows folder (ex: ester.txt)
CD bin\Debug\net8.0-windows

:: pause 30 sec
echo "wait 30 sec before start ..."
timeout /t 30 /nobreak

WorstTradingInitiative.exe COLLECT

:: pause
