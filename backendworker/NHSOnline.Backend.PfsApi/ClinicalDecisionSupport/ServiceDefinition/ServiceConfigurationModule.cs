using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Mappers;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
using NHSOnline.Backend.Support;
using Name = NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models.Name;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public class ServiceConfigurationModule : NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IServiceDefinitionService, ServiceDefinitionService>();
            services.AddSingleton<IMapper<DemographicsResponse, OlcDemographics>,
                OlcDemographicsMapper>();
            services.AddSingleton<IMapper<string, DemographicsName, Name>,
                OlcDemographicsNameMapper>();
            services.AddSingleton<IOlcDataMaps, OlcDataMaps>();
            services.AddTransient<ICreateFhirParameter, CreateFhirParameter>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}