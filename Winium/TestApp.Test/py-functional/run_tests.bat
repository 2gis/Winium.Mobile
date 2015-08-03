echo %RELEASE%
IF [%RELEASE%] == [1] (SET CONFIGURATION=Release) else (SET CONFIGURATION=Debug) 

echo %CONFIGURATION%

REM Run Winium.StoreApps.Driver.exe
start ..\..\Winium.StoreApps.Driver\bin\x86\%CONFIGURATION%\Winium.StoreApps.Driver.exe

REM Run tests
pip install -r requirements.txt
py.test tests --tb=native -s --junitxml=junit-result.xml
SET RV=%ERRORLEVEL%

taskkill /im Winium.StoreApps.Driver.exe /f

EXIT %RV% /B