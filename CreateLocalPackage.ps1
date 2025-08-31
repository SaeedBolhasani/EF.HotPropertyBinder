#!/usr/bin/env pwsh

# Create Local NuGet Package Script
# This script will build and create a local NuGet package for EF.HotPropertyBinder

Write-Host "Creating local NuGet package for EF.HotPropertyBinder..." -ForegroundColor Green

# Create output directory for packages
$packageDir = ".\LocalPackages"
if (!(Test-Path $packageDir)) {
    New-Item -ItemType Directory -Path $packageDir -Force
    Write-Host "Created directory: $packageDir" -ForegroundColor Yellow
}

# Build and pack the project
Write-Host "Building and packing the project..." -ForegroundColor Yellow
dotnet pack "EF.HotPropertyBinder\EF.HotPropertyBinder.csproj" --configuration Release --output $packageDir --verbosity normal

if ($LASTEXITCODE -eq 0) {
    Write-Host "Package created successfully!" -ForegroundColor Green
    
    # List created packages
    $packages = Get-ChildItem -Path $packageDir -Filter "*.nupkg"
    Write-Host "Created packages:" -ForegroundColor Cyan
    foreach ($package in $packages) {
        Write-Host "  - $($package.Name)" -ForegroundColor White
    }
    
    # Add local package source if not exists
    $sourceName = "LocalPackages"
    $existingSource = dotnet nuget list source | Select-String $sourceName
    
    if (-not $existingSource) {
        $fullPath = Resolve-Path $packageDir
        Write-Host "Adding local NuGet source: $sourceName" -ForegroundColor Yellow
        dotnet nuget add source $fullPath --name $sourceName
    } else {
        Write-Host "Local NuGet source '$sourceName' already exists" -ForegroundColor Yellow
    }
    
    Write-Host "`nTo use this package in another project, run:" -ForegroundColor Cyan
    Write-Host "dotnet add package EF.HotPropertyBinder --source LocalPackages" -ForegroundColor White
    
} else {
    Write-Host "Failed to create package. Exit code: $LASTEXITCODE" -ForegroundColor Red
}

Write-Host "`nPackage creation script completed." -ForegroundColor Green