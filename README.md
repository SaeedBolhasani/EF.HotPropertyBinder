# EF.HotPropertyBinder

A powerful Entity Framework Core extension that provides automatic dependency injection into entity properties using attributes and source generation.

## Features

- ?? **Hot Property Binding**: Automatically inject services into entity properties when materialized from the database
- ?? **Source Generation**: High-performance code generation for optimal runtime performance
- ?? **Attribute-Based**: Simple `[HotBind]` attribute to mark properties for injection
- ?? **EF Core Integration**: Seamless integration with Entity Framework Core materialization interceptors

## Installation

```bash
dotnet add package EF.HotPropertyBinder
```

## Quick Start

### 1. Mark Properties with HotBind Attribute

```csharp
public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    
    [HotBind]
    public IOrderService OrderService { get; set; }
    
    [HotBind]
    public INotificationService NotificationService { get; set; }
}
```

### 2. Register Services and Interceptor

```csharp
services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(connectionString)
           .AddInterceptors(serviceProvider.GetRequiredService<HotPropertyMaterializationInterceptor>());
});

services.AddScoped<HotPropertyMaterializationInterceptor>();
services.AddScoped<IOrderService, OrderService>();
services.AddScoped<INotificationService, NotificationService>();
```

### 3. Use Your Entities

```csharp
var orders = await context.Orders.ToListAsync();
// Properties marked with [HotBind] are automatically injected!
foreach (var order in orders)
{
    order.OrderService.ProcessOrder(); // Ready to use!
    order.NotificationService.SendConfirmation();
}
```

## How It Works

1. **Compile Time**: Source generator analyzes your entities and creates optimized binding code
2. **Runtime**: Materialization interceptor automatically injects services when entities are loaded from database
3. **Performance**: Generated code provides near-zero overhead compared to manual injection

## Requirements

- .NET 8.0 or later
- Entity Framework Core 8.0 or later

## License

MIT License - see LICENSE file for details.