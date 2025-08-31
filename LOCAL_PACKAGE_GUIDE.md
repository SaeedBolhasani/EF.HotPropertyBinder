# Creating Local NuGet Package for EF.HotPropertyBinder

## Package Configuration

The project has been configured for NuGet packaging with the following features:

### Package Metadata
- **PackageId**: EF.HotPropertyBinder
- **Version**: 1.0.0
- **Title**: EF Hot Property Binder
- **Description**: Entity Framework Core extension for automatic dependency injection into entity properties using attributes and source generation.
- **Tags**: EntityFramework, EF, EFCore, DependencyInjection, SourceGenerator, Attributes
- **License**: MIT
- **Auto-build**: Enabled (`GeneratePackageOnBuild=true`)

## How to Create the Package

### Method 1: Using dotnet pack
```bash
dotnet pack EF.HotPropertyBinder/EF.HotPropertyBinder.csproj --configuration Release --output ./LocalPackages
```

### Method 2: Using MSBuild
```bash
msbuild EF.HotPropertyBinder/EF.HotPropertyBinder.csproj /p:Configuration=Release /t:Pack
```

### Method 3: Automatic during build
Since `GeneratePackageOnBuild=true` is set, the package will be created automatically when you build the project:
```bash
dotnet build EF.HotPropertyBinder/EF.HotPropertyBinder.csproj --configuration Release
```

## Setting Up Local NuGet Source

### Add Local Package Source
```bash
# Create a local packages directory
mkdir LocalPackages

# Add it as a NuGet source
dotnet nuget add source "./LocalPackages" --name "LocalPackages"
```

### List NuGet Sources
```bash
dotnet nuget list source
```

### Remove Local Source (if needed)
```bash
dotnet nuget remove source "LocalPackages"
```

## Using the Local Package

### In a New Project
```bash
# Create a new project
dotnet new console -n TestProject
cd TestProject

# Add reference to local package
dotnet add package EF.HotPropertyBinder --source LocalPackages

# Or specify version
dotnet add package EF.HotPropertyBinder --version 1.0.0 --source LocalPackages
```

### Package Location
The package files will be created in:
- **Debug builds**: `EF.HotPropertyBinder/bin/Debug/`
- **Release builds**: `EF.HotPropertyBinder/bin/Release/`
- **Custom output**: Location specified by `--output` parameter

## Package Contents

The package includes:
- ? Main assembly (EF.HotPropertyBinder.dll)
- ? Source generator (HotBindSourceGenerator.cs)
- ? Symbol files (.pdb) for debugging
- ? Dependencies metadata
- ? Package metadata and documentation

## Troubleshooting

### Package Not Created
1. Ensure `GeneratePackageOnBuild=true` is set in the project file
2. Build the project in Release configuration
3. Check the bin/Release folder for .nupkg files

### Cannot Find Package
1. Verify the local source is added: `dotnet nuget list source`
2. Check package exists in the source directory
3. Ensure package ID and version match exactly

### Source Generator Not Working
1. Verify `HotBindSourceGenerator.cs` is included as `<Analyzer>`
2. Check that `HOTBIND_GENERATED` symbol is defined
3. Ensure target project references the package correctly

## Project Structure for Package
```
EF.HotPropertyBinder/
??? EF.HotPropertyBinder.csproj     # Package configuration
??? HotBindAttribute.cs             # Core attribute
??? MaterializationInterceptor.cs   # EF Core interceptor
??? HotBindSourceGenerator.cs       # Source generator (if exists)
??? Generated/
?   ??? HotBindHelper.cs           # Generated helper code
??? Examples/
    ??? ExampleEntity.cs           # Usage examples
```