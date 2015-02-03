REM delete existing nuget packages
del *.nupkg
set NUGET=.\.nuget\nuget.exe
%NUGET% pack  .\WindowsPhoneDriver.InnerDriver\WindowsPhoneDriver.InnerDriver.csproj -IncludeReferencedProjects -Prop Configuration=Release
%NUGET% push *.nupkg