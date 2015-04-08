REM Run Winium.StoreApps.Driver.exe
start ..\..\Winium.StoreApps.Driver\bin\x86\Release\Winium.StoreApps.Driver.exe

REM Run tests
pip install -r requirements.txt
py.test tests --tb=native -s --junitxml=junit-result.xml

taskkill /im Winium.StoreApps.Driver.exe /f
taskkill /im XDE.exe /f