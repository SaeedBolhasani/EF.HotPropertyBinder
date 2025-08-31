using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EF.HotPropertyBinder
{
    /// <summary>
    /// Extension methods for IServiceCollection to register HotBind functionality
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds HotBind services to the service collection, enabling automatic dependency injection
        /// into entity properties marked with [HotBind] attribute.
        /// Note: Source generation is automatically enabled when you reference this package.
        /// </summary>
        /// <param name="services">The service collection to add services to</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddHotBind(this IServiceCollection services)
        {
            // Register the materialization interceptor as scoped
            services.TryAddScoped<HotPropertyMaterializationInterceptor>();
            
            return services;
        }

        /// <summary>
        /// Adds HotBind services and configures Entity Framework DbContext to use the interceptor.
        /// This is a convenience method that combines AddHotBind() with DbContext configuration.
        /// Source generation will automatically create optimized binding code for entities with [HotBind] properties.
        /// </summary>
        /// <typeparam name="TContext">The DbContext type</typeparam>
        /// <param name="services">The service collection to add services to</param>
        /// <param name="optionsAction">Configuration action for DbContext options</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddHotBindDbContext<TContext>(
            this IServiceCollection services, 
            Action<DbContextOptionsBuilder> optionsAction)
            where TContext : DbContext
        {
            // Add HotBind services
            services.AddHotBind();

            // Configure DbContext with HotBind interceptor
            services.AddDbContext<TContext>((serviceProvider, options) =>
            {
                // Apply user configuration first
                optionsAction(options);
                
                // Add the HotBind interceptor
                var interceptor = serviceProvider.GetRequiredService<HotPropertyMaterializationInterceptor>();
                options.AddInterceptors(interceptor);
            });

            return services;
        }

        /// <summary>
        /// Adds HotBind interceptor to an existing DbContext configuration.
        /// Use this when you already have a DbContext configured and just want to add HotBind support.
        /// </summary>
        /// <param name="optionsBuilder">The DbContext options builder</param>
        /// <param name="serviceProvider">The service provider to resolve the interceptor from</param>
        /// <returns>The options builder for method chaining</returns>
        public static DbContextOptionsBuilder UseHotBind(
            this DbContextOptionsBuilder optionsBuilder, 
            IServiceProvider serviceProvider)
        {
            var interceptor = serviceProvider.GetRequiredService<HotPropertyMaterializationInterceptor>();
            return optionsBuilder.AddInterceptors(interceptor);
        }

        /// <summary>
        /// Validates that HotBind is properly configured in the service collection.
        /// This method can be used for debugging or testing to ensure all required services are registered.
        /// </summary>
        /// <param name="services">The service collection to validate</param>
        /// <returns>True if HotBind is properly configured, false otherwise</returns>
        public static bool IsHotBindConfigured(this IServiceCollection services)
        {
            return services.Any(descriptor => 
                descriptor.ServiceType == typeof(HotPropertyMaterializationInterceptor));
        }
    }
}