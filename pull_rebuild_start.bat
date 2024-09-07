:: kill

taskkill /F /IM WorstTradingInitiative.exe

:: git pull, do we git stash ?
"C:\Program Files\Git\bin\sh.exe" --login -i -c "git pull"


:: build
SET SLN=WorstTradingInitiative.sln
SET MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe
"%MSBUILD%" "%SLN%" /p:Configuration=Debug

:: we change path to have WorstTradingInitiative.exe output everything in bin\Debug\net8.0-windows folder (ex: ester.txt)
CD bin\Debug\net8.0-windows

WorstTradingInitiative.exe

:: pause
