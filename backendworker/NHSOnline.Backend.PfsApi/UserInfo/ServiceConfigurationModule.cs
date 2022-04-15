using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.PfsApi.Areas.UserInfo.Mappers;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UserInfo.Areas.UserInfo.Models;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMapper<CitizenIdSessionResult, InfoUserProfile>, InfoUserProfileMapper>();

            base.ConfigureServices(services, configuration);
        }
    }
}