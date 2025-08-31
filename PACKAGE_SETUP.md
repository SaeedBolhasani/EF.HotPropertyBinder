# EF.HotPropertyBinder Local Package Setup

## Quick Start
Your project is now configured as a local NuGet package!

### Create the Package
```bash
dotnet pack EF.HotPropertyBinder/EF.HotPropertyBinder.csproj --configuration Release --output ./packages
```

### Add Local Source
```bash
dotnet nuget add source "./packages" --name "LocalPackages"
```

### Use in Another Project
```bash
dotnet add package EF.HotPropertyBinder --source LocalPackages
```

## Package Details
- **ID**: EF.HotPropertyBinder
- **Version**: 1.0.0
- **Auto-build**: Enabled
- **Symbols**: Included for debugging

The package will be created automatically when building in Release mode due to `GeneratePackageOnBuild=true` setting.