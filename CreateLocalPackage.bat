@echo off
echo Creating local NuGet package for EF.HotPropertyBinder...

REM Create output directory for packages
if not exist "LocalPackages" (
    mkdir LocalPackages
    echo Created directory: LocalPackages
)

REM Build and pack the project
echo Building and packing the project...
dotnet pack "EF.HotPropertyBinder\EF.HotPropertyBinder.csproj" --configuration Release --output LocalPackages --verbosity normal

if %ERRORLEVEL% EQU 0 (
    echo Package created successfully!
    
    REM List created packages
    echo Created packages:
    dir LocalPackages\*.nupkg /B
    
    REM Add local package source if not exists
    echo Adding local NuGet source...
    dotnet nuget add source "%CD%\LocalPackages" --name LocalPackages 2>nul
    
    echo.
    echo To use this package in another project, run:
    echo dotnet add package EF.HotPropertyBinder --source LocalPackages
    
) else (
    echo Failed to create package. Error code: %ERRORLEVEL%
)

echo.
echo Package creation script completed.
pause