Function Build()
{
    $msbuild=$env:MSBUILD
    $solutionPath = Join-Path $solutionDir 'Winium.sln'

    &$msbuild ($solutionPath, '/v:minimal', '/p:configuration=Release', '/t:Clean,Build')
    if (!$?) {
        Write-Host "Build failed. $?" -ForegroundColor Red
        Exit 1
    }
}

Function Test()
{
    Write-Host "Running tests"
    $runTestsPath = Join-Path $solutionDir 'TestApp.Test\py-functional'

    $env:RELEASE=1
    $env:REMOTE_RUN=1
    $proc = Start-Process "$runTestsPath/run_tests.bat" -PassThru -Wait -WorkingDirectory $runTestsPath
    Write-Host $proc.ExitCode

    if (!$proc.ExitCode.Equals(0))
    {
        Write-Host "Tests failed. See junit-result.xml" -ForegroundColor Red
        Exit 2
    }
}

Function Clean()
{
    Remove-Item $releaseDir -Force -Recurse
    New-Item -ItemType directory -Path $releaseDir
}

Function PackNuGet ()
{
    Write-Host "Packing Nuget package"
    Get-ChildItem -Path $releaseDir -Filter "*.nupkg" | foreach ($_) { Remove-Item $_.FullName }

    $nuget = Join-Path $solutionDir '.nuget\nuget.exe'

    $innerServerProjectDir = Join-Path $solutionDir 'Winium.StoreApps.InnerServer'
    $innerServerprojectPath = Join-Path $innerServerProjectDir 'Winium.StoreApps.InnerServer.csproj'

    &$nuget ('pack', $innerServerprojectPath, '-IncludeReferencedProjects', '-Properties', 'Configuration=Release', '-OutputDirectory', $releaseDir)
}

Function PackRelease()
{
    Add-Type -assembly "system.io.compression.filesystem"

    $driverSourcePath = Join-Path $solutionDir "Winium.StoreApps.Driver\bin\x86\Release"
    $innerServerSourcePath = Join-Path $solutionDir "Winium.StoreApps.InnerServer\bin\Release"
    $inspectorSourcePath = Join-Path $solutionDir "Winium.StoreApps.Inspector\bin\Release"

    Get-ChildItem -Path $releaseDir -Filter "*.zip" | foreach ($_) { Remove-Item $_.FullName }

    [IO.Compression.ZipFile]::CreateFromDirectory($driverSourcePath, "$releaseDir/Winium.StoreApps.Driver.zip")
    [IO.Compression.ZipFile]::CreateFromDirectory($innerServerSourcePath, "$releaseDir/Winium.StoreApps.InnerServer.zip")
    [IO.Compression.ZipFile]::CreateFromDirectory($inspectorSourcePath, "$releaseDir/Winium.StoreApps.Inspector.zip")
}

$workspace=$PSScriptRoot
$releaseDir = Join-Path $workspace "Release"
$solutionDir=Join-Path $workspace "Winium"

Write-Host "Update CHANGELOG.md"
Write-Host "Update version in Assemblies"
Write-Host "Update version in NuSpec file"

Pause

Clean
Build
Test
PackNuGet
PackRelease

Write-Host "Finished" -ForegroundColor Green
Write-Host "Publish NuGet package using nuget.exe push $releaseDir\Winium.StoreApps.InnerServer.*.nupkg"
Write-Host "Add and push tag using git tag -a v*.*.* -m 'Version *.*.*'"
Write-Host "Upload and attach $releaseDir\*.zip files to release"
