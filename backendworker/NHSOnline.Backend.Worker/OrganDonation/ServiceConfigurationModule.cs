using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        private readonly ILogger<ServiceConfigurationModule> _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<OrganDonationHttpRequestIdentifier>();
            services.AddSingleton<IOrganDonationConfig, OrganDonationConfig>();

            if (bool.TrueString.Equals(
                configuration.GetOrWarn("ORGAN_DONATION_INTEGRATION_ENABLED", _logger),
                StringComparison.OrdinalIgnoreCase))
            {
                services.AddSingleton<IOrganDonationClient, OrganDonationClient>();

                services.AddHttpClient<OrganDonationHttpClient>()
                    .AddHttpMessageHandler<HttpTimeoutHandler<OrganDonationHttpRequestIdentifier>>()
                    .AddHttpMessageHandler<HttpRequestIdentificationHandler<OrganDonationHttpRequestIdentifier>>();
            }
            else
            {
                services.AddSingleton<IOrganDonationClient, OrganDonationMockClient>();
            }

            services.AddSingleton<IOrganDonationService, OrganDonationService>();

            services.AddSingleton<IMapper<DemographicsResponse, OrganDonationRegistration>,
                OrganDonationRegistrationMapper>();

            services
                .AddSingleton<IMapper<OrganDonationRegistration,
                        OrganDonationSuccessResponse<RegistrationLookupResponse>, OrganDonationRegistration>,
                    OrganDonationRegistrationMapper>();

            services.AddSingleton<IMapper<OrganDonationRegistration, LookupRegistrationRequest>,
                LookupRegistrationRequestMapper>();

            services
                .AddSingleton<IMapper<OrganDonationResponse<ReferenceDataResponse>, OrganDonationReferenceDataResponse>,
                    OrganDonationReferenceDataResponseMapper>();

            services.AddSingleton<IMapper<string, Decision>, OrganDonationDecisionMapper>();
            services.AddSingleton<IMapper<string, ChoiceState>, OrganDonationChoiceStateMapper>();
            services.AddSingleton<IMapper<string, FaithDeclaration>, OrganDonationFaithDeclarationMapper>();

            base.ConfigureServices(services, configuration);
        }
    }
}