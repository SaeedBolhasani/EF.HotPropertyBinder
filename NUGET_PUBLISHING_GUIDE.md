# Publishing EF.HotPropertyBinder to NuGet.org

## ?? Pre-Publishing Checklist

Your project is already well-configured! Here's what you have ready:

### ? Package Metadata (Complete)
- **Package ID**: `EF.HotPropertyBinder`
- **Version**: `1.0.0`
- **Author**: `Saeed Bolhasani`
- **Description**: Comprehensive and clear
- **Tags**: Well-defined for discoverability
- **License**: MIT (appropriate for open source)
- **Repository URLs**: Configured for GitHub
- **README**: Well-written documentation

### ? Technical Requirements (Ready)
- Target Framework: `.NET 8.0`
- Source Generator: Included as analyzer
- Dependencies: Properly configured
- Symbol packages: Enabled (`.snupkg`)

## ?? Step-by-Step Publishing Guide

### Step 1: Create NuGet.org Account
1. Go to [nuget.org](https://www.nuget.org)
2. Sign up/Sign in with Microsoft, GitHub, or create account
3. Verify your email address

### Step 2: Generate API Key
1. Go to [nuget.org/account/apikeys](https://www.nuget.org/account/apikeys)
2. Click **"Create"**
3. Configure the API key:
   - **Key Name**: `EF.HotPropertyBinder Publishing`
   - **Package Owner**: Select your account
   - **Scopes**: Select **"Push new packages and package versions"**
   - **Packages**: Select **"All packages"** or specify `EF.HotPropertyBinder`
   - **Expiration**: Choose appropriate duration (e.g., 365 days)
4. Click **"Create"**
5. **Copy and save the API key securely** (you won't see it again)

### Step 3: Build and Create Package
```bash
# Clean and build in Release mode
dotnet clean EF.HotPropertyBinder/EF.HotPropertyBinder.csproj
dotnet build EF.HotPropertyBinder/EF.HotPropertyBinder.csproj --configuration Release

# Create the package
dotnet pack EF.HotPropertyBinder/EF.HotPropertyBinder.csproj --configuration Release --output ./packages
```

### Step 4: Publish to NuGet.org
```bash
# Navigate to packages directory
cd packages

# Publish the package (replace YOUR_API_KEY with your actual key)
dotnet nuget push EF.HotPropertyBinder.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# If you have symbol packages, publish them too
dotnet nuget push EF.HotPropertyBinder.1.0.0.snupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

### Step 5: Verify Publication
1. Go to [nuget.org/packages/EF.HotPropertyBinder](https://www.nuget.org/packages/EF.HotPropertyBinder)
2. Check that package information is correct
3. Test installation: `dotnet add package EF.HotPropertyBinder`

## ?? Alternative Publishing Methods

### Using Visual Studio
1. Right-click project ? **"Pack"**
2. Go to `bin/Release/` folder
3. Right-click `.nupkg` file ? **"Push to NuGet.org"**
4. Enter your API key when prompted

### Using NuGet CLI
```bash
# If you have nuget.exe installed
nuget push EF.HotPropertyBinder.1.0.0.nupkg -ApiKey YOUR_API_KEY -Source https://api.nuget.org/v3/index.json
```

## ?? Package Validation

Before publishing, validate your package:

```bash
# Install dotnet package validation tool
dotnet tool install -g Microsoft.DotNet.PackageValidation.Cli

# Validate your package
dotnet package-validation EF.HotPropertyBinder.1.0.0.nupkg
```

## ?? Security Best Practices

### API Key Security
- **Never commit API keys to source control**
- Store in environment variables or secure vaults
- Use scoped keys (not global)
- Rotate keys regularly

### Environment Variable Setup
```bash
# Windows (PowerShell)
$env:NUGET_API_KEY = "your-api-key-here"
dotnet nuget push EF.HotPropertyBinder.1.0.0.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json

# Linux/macOS
export NUGET_API_KEY="your-api-key-here"
dotnet nuget push EF.HotPropertyBinder.1.0.0.nupkg --api-key $NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

## ?? Future Updates

### Version Management
Update the version in your `.csproj` file for new releases:
```xml
<PackageVersion>1.0.1</PackageVersion>
```

### Semantic Versioning
- **Major** (1.0.0 ? 2.0.0): Breaking changes
- **Minor** (1.0.0 ? 1.1.0): New features, backward compatible
- **Patch** (1.0.0 ? 1.0.1): Bug fixes, backward compatible

### Release Notes
Consider adding release notes:
```xml
<PackageReleaseNotes>
- Initial release
- Hot property binding with [HotBind] attribute
- Source generator for optimal performance
- EF Core 8.0 materialization interceptor
</PackageReleaseNotes>
```

## ??? Troubleshooting

### Common Issues
1. **"Package already exists"**: Increment version number
2. **"Invalid API key"**: Check key scope and expiration
3. **"Package validation failed"**: Run package validation tool
4. **"Missing required metadata"**: Ensure all required properties are set

### Package Not Appearing
- Wait 10-15 minutes for indexing
- Check package status on nuget.org
- Verify package wasn't rejected due to policy violations

## ?? After Publishing

### Monitor Usage
- Check download statistics on nuget.org
- Monitor GitHub issues and discussions
- Update documentation based on user feedback

### Package Management
- Set up CI/CD for automated publishing
- Configure branch protection for releases
- Consider pre-release versions for testing

Your package is ready for publishing! The metadata and structure look excellent for a successful NuGet.org release.