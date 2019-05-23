using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Demographics.Models;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Demographics
{
    public class ServiceConfigurationModule : Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDemographicsResultVisitor<IActionResult>, DemographicsResultVisitor>();
            services.AddSingleton<IMapper<DemographicsResult.Success, SuccessfulDemographicsResult>,
                SuccessfulDemographicsResultMapper>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}