using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.HttpMocks.Vision
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection RegisterVisionService(this IServiceCollection serviceCollection)
        {
            foreach (var handler in Assembly.GetExecutingAssembly().DefinedTypes.Where(t => t.IsClass).Where(typeof(IVisionSoapRequestHandler).IsAssignableFrom))
            {
                serviceCollection.AddTransient(typeof(IVisionSoapRequestHandler), handler);
            }

            return serviceCollection;
        }
    }
}