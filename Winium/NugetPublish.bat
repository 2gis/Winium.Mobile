REM delete existing nuget packages
del *.nupkg
set NUGET=.\.nuget\nuget.exe
%NUGET% pack .\Winium.StoreApps.InnerServer\Winium.StoreApps.InnerServer.csproj -IncludeReferencedProjects -Prop Configuration=Release
%NUGET% pack .\Winium.Silverlight.InnerServer\Winium.Silverlight.InnerServer.csproj -IncludeReferencedProjects -Prop Configuration=Release
pause
%NUGET% push *.nupkg
