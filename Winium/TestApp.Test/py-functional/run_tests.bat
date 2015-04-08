REM Run Winium.StoreApps.Driver.exe
start ..\..\Winium.StoreApps.Driver\bin\x86\Debug\Winium.StoreApps.Driver.exe

REM Run tests
pip install -r requirements.txt
py.test tests --tb=native -s

taskkill /im Winium.StoreApps.Driver.exe /f