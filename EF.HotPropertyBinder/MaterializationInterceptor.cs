using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
#if HOTBIND_GENERATED
using EF.HotPropertyBinder.Generated;
#endif

namespace EF.HotPropertyBinder
{
    /// <summary>
    /// Materialization interceptor that catches objects when creating entities from database queries
    /// </summary>
    public class HotPropertyMaterializationInterceptor : IMaterializationInterceptor
    {
        private readonly IServiceProvider _serviceProvider;

        public HotPropertyMaterializationInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public object InitializedInstance(MaterializationInterceptionData materializationData, object entity)
        {
            // Example: auto-initialize or transform some data
            if (entity is not null)
            {
#if HOTBIND_GENERATED
                // Use the generated helper for better performance
                HotBindHelper.BindHotProperties(entity, _serviceProvider);
#endif
            }
            return entity;
        }
    }
}