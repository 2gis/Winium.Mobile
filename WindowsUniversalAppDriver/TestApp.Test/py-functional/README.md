# Functional tests for Windows Universal App Driver
Functional tests for Windows Universal App Driver written in python.

## Usage

1. Build Windows Universal App Driver solution.
2. Make store package from TestApp.
3. Run tests

```cmd
REM Run WindowsUniversalAppDriver.exe
start ..\..\WindowsUniversalAppDriver\bin\Debug\WindowsUniversalAppDriver.exe

REM Run tests
pip install -r requirements.txt
py.test tests --tb=native -s
```