using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.Areas.Demographics.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.Areas.Demographics
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDemographicsResultVisitor<IActionResult>, DemographicsResultVisitor>();
            services.AddSingleton<IMapper<DemographicsResult.SuccessfullyRetrieved, SuccessfulDemographicsResult>,
                SuccessfulDemographicsResultMapper>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}