echo %RELEASE%
IF [%RELEASE%] == [1] (SET CONFIGURATION=Release) else (SET CONFIGURATION=Debug)

echo %CONFIGURATION%

REM Run Winium.Mobile.Driver.exe
start ..\..\Winium.Mobile.Driver\bin\x86\%CONFIGURATION%\Winium.Mobile.Driver.exe

REM Run tests
pip install -r requirements.txt
py.test tests --tb=native -s --junitxml=junit-result-storeapps.xml
SET RV1=%ERRORLEVEL%
py.test tests_silverlight --tb=native -s --junitxml=junit-result-silverlight.xml
SET RV2=%ERRORLEVEL%

taskkill /im Winium.Mobile.Driver.exe /f

SET /A RV=%RV1%+%RV2%
EXIT %RV% /B
