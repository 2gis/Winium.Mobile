REM delete existing nuget packages
del *.nupkg
set NUGET=.\.nuget\nuget.exe
%NUGET% pack .\WindowsUniversalAppDriver.InnerServer\WindowsUniversalAppDriver.InnerServer.csproj -IncludeReferencedProjects -Prop Configuration=Release
%NUGET% push *.nupkg