using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public class ServiceConfigurationModule : Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
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
    }
}