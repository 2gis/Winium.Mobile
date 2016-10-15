SET CONFIGURATION=Release
echo %CONFIGURATION%

REM Run Winium.Mobile.Driver.exe
start ..\..\Winium.Mobile.Driver\bin\x86\Release\Winium.Mobile.Driver.exe

REM Run tests
pip install -r requirements.txt
py.test tests --tb=native -s --junitxml=junit-result-storeapps.xml
py.test tests_silverlight --tb=native -s --junitxml=junit-result-silverlight.xml

taskkill /im Winium.Mobile.Driver.exe /f
taskkill /im XDE.exe /f
