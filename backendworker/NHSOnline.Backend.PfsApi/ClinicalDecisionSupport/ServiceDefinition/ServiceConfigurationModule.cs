using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Mappers;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using Name = NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models.Name;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureValidatableSetting<OnlineConsultationsProvidersSettings>(configuration.GetSection("OnlineConsultationsProvidersSettings"));

            services.AddHttpContextAccessor();
            services.AddSingleton<IFhirSanitizationHelper, FhirSanitizationHelper>();

            services.AddTransient<OnlineConsultationsHttpRequestIdentifier>();

            var config = configuration.GetSection("OnlineConsultationsProvidersSettings").Get<OnlineConsultationsProvidersSettings>();

            foreach (var providerSettings in config.Providers) {
                services.AddHttpClient(providerSettings.Provider, httpClient =>
                    httpClient.BaseAddress = new Uri(providerSettings.BaseAddress))
                    .AddHttpMessageHandler<HttpTimeoutHandler<OnlineConsultationsHttpRequestIdentifier>>()
                    .AddHttpMessageHandler<HttpRequestIdentificationHandler<OnlineConsultationsHttpRequestIdentifier>>();
            }

            services.AddSingleton<IEvaluateServiceDefinitionQuery, EvaluateServiceDefinitionQuery>();
            services.AddSingleton<IServiceDefinitionIsValidQuery, ServiceDefinitionIsValidQuery>();

            services.AddTransient<IServiceDefinitionService, ServiceDefinitionService>();
            services.AddTransient<ServiceDefinitionQuerySender>();
            services.AddSingleton<IOlcDataMaps, OlcDataMaps>();
            services.AddTransient<IFhirParameterHelpers, FhirParameterHelpers>();

            base.ConfigureServices(services, configuration);
        }
    }
}