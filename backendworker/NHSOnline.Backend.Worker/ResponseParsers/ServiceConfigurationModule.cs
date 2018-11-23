using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.ResponseParsers
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IJsonResponseParser, JsonResponseParser>();
            services.AddSingleton<IXmlResponseParser, XmlResponseParser>();
            base.ConfigureServices(services, configuration);
        }
    }
}
