using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        private readonly ILogger _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var termsAndConditionsConfiguration = CreateTermsAndConditionsConfiguration(configuration);
            services.AddSingleton(termsAndConditionsConfiguration);

            services.AddTransient<IMapper<TermsAndConditionsRecord, ConsentResponse>, TermsAndConditionsToConsentResponseMapper>();
            services.AddTransient<IConsentRequestToTermsAndConditionsMapper, ConsentRequestToTermsAndConditionsMapper>();
            services.AddTransient<IMapper<ConsentRequest, DateTimeOffset, UpdateRecordBuilder<TermsAndConditionsRecord>> ,ConsentRequestToUpdateMapper>();
            services
                .AddTransient<
                    IMapper<AnalyticsCookieAcceptance, DateTimeOffset, UpdateRecordBuilder<TermsAndConditionsRecord>>,
                    AnalyticsCookieAcceptanceToUpdateMapper>();

            services.RegisterRepository<TermsAndConditionsRecord, ConsentRepositoryConfiguration>();
            services.AddTransient<IMapper<TermsAndConditionsRecord, ConsentResponse>, TermsAndConditionsToConsentResponseMapper>();
            services.AddTransient<ITermsAndConditionsService, TermsAndConditionsService>();
            services.AddTransient<ITermsAndConditionsRepository, TermsAndConditionsRepository>();

            base.ConfigureServices(services, configuration);
        }

        private ITermsAndConditionsConfiguration CreateTermsAndConditionsConfiguration(IConfiguration configuration)
        {
            var effectiveDate = configuration.GetOrThrow("ConfigurationSettings:CurrentTermsConditionsEffectiveDate", _logger);
            return new TermsAndConditionsConfiguration(effectiveDate);
        }
    }
}