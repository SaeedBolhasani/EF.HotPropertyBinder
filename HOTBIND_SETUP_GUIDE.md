# How to Enable HOTBIND_GENERATED Source Generation

When you install the `EF.HotPropertyBinder` NuGet package in your project, the source generator should **automatically** run and generate optimized binding code. Here's how it works and how to ensure it's properly enabled:

## ? Automatic Source Generation

### What Happens Automatically:
1. **Source Generator Runs**: The `HotBindSourceGenerator` analyzes your code during compilation
2. **Code Generation**: Creates optimized `HotBindHelper.g.cs` with binding logic for your entities
3. **Symbol Definition**: The generated code enables the `HOTBIND_GENERATED` conditional compilation blocks
4. **Performance Optimization**: Uses generated code instead of reflection-based binding

### In Your Consumer Project:

#### 1. Install the Package
```bash
dotnet add package EF.HotPropertyBinder
```

#### 2. Create Your Entities
```csharp
public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    
    [HotBind]  // This triggers source generation
    public IOrderService OrderService { get; set; }
    
    [HotBind]
    public INotificationService NotificationService { get; set; }
}
```

#### 3. Use the Extension Method
```csharp
// In Program.cs or Startup.cs
services.AddHotBindDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register your services
services.AddScoped<IOrderService, OrderService>();
services.AddScoped<INotificationService, NotificationService>();
```

## ?? Verifying Source Generation

### Check Generated Files:
1. **Build your project**: `dotnet build`
2. **Look for generated files** in your IDE (usually under Dependencies > Analyzers)
3. **Check obj folder**: Look for generated files in `obj/Debug/net8.0/generated/`

### Expected Generated Files:
- `HotBindHelper.g.cs` - Contains optimized binding logic
- `DebugInfo.g.cs` - Confirms the generator is running

### Visual Studio:
- Right-click your project ? **Show All Files**
- Expand **Dependencies** ? **Analyzers** ? **EF.HotPropertyBinder**
- You should see the generated files

## ??? Troubleshooting

### Source Generation Not Working?

#### 1. **Check Package Installation**
```xml
<!-- Verify in your .csproj -->
<PackageReference Include="EF.HotPropertyBinder" Version="1.0.0" />
```

#### 2. **Enable Source Generator Logs**
Add to your project file to see generator diagnostics:
```xml
<PropertyGroup>
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  <CompilerGeneratedFilesOutputPath>Generated</CompilerGeneratedFilesOutputPath>
</PropertyGroup>
```

#### 3. **Manual Symbol Definition** (Fallback)
If source generation isn't working, you can manually enable it:
```xml
<PropertyGroup>
  <DefineConstants>$(DefineConstants);HOTBIND_GENERATED</DefineConstants>
</PropertyGroup>
```

#### 4. **Clean and Rebuild**
```bash
dotnet clean
dotnet build
```

### Common Issues:

#### ? "HotBindHelper not found"
- **Cause**: Source generator not running
- **Solution**: Check package reference and rebuild project

#### ? "HOTBIND_GENERATED not defined"
- **Cause**: Generated files not included in compilation
- **Solution**: Enable EmitCompilerGeneratedFiles or define symbol manually

#### ? "Source generator not executing"
- **Cause**: Analyzer not properly included in package
- **Solution**: Verify package contains analyzers folder

## ?? Generated Code Structure

When working correctly, you'll see generated code like this:

```csharp
// Generated: HotBindHelper.g.cs
using System;
using Microsoft.Extensions.DependencyInjection;

namespace EF.HotPropertyBinder.Generated
{
    public static class HotBindHelper
    {
        public static void BindHotProperties(object entity, IServiceProvider serviceProvider)
        {
            switch (entity)
            {
                case YourNamespace.Order orderEntity:
                    orderEntity.OrderService = (IOrderService)serviceProvider.GetRequiredService<IOrderService>();
                    orderEntity.NotificationService = (INotificationService)serviceProvider.GetRequiredService<INotificationService>();
                    break;
                
                default:
                    break;
            }
        }
    }
}
```

## ?? Best Practices

### For Library Authors:
- Include source generators as analyzers in NuGet package
- Use `PrivateAssets="all"` for analyzer dependencies
- Test source generation in consumer projects

### For Consumer Projects:
- Keep entity classes simple with [HotBind] attributes
- Register all services that entities depend on
- Use the provided extension methods for easy setup
- Verify source generation during development

## ?? Performance Benefits

When `HOTBIND_GENERATED` is enabled:
- ? **Compile-time optimization**: No reflection at runtime
- ? **Type safety**: Generated code is strongly typed
- ? **Better performance**: Direct service resolution
- ? **Debugging**: Generated code is debuggable

The source generator automatically enables these optimizations when it detects entities with `[HotBind]` attributes in your project!