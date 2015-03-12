REM Run WindowsUniversalAppDriver.exe
start ..\..\WindowsUniversalAppDriver\bin\Debug\WindowsUniversalAppDriver.exe

REM Run tests
pip install -r requirements.txt
py.test tests --tb=native -s