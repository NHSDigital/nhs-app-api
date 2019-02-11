using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Http;
using Address = NHSOnline.Backend.Worker.OrganDonation.ApiModels.Address;
using Name = NHSOnline.Backend.Worker.OrganDonation.ApiModels.Name;

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
                services.AddSingleton<OrganDonationHttpClientHandler>();

                services.AddHttpClient<OrganDonationHttpClient>()
                    .ConfigurePrimaryHttpMessageHandler<OrganDonationHttpClientHandler>()
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
                .AddSingleton<IMapper<OrganDonationRegistration, RegistrationLookupResponse, OrganDonationRegistration>,
                    OrganDonationRegistrationMapper>();

            services.AddSingleton<IMapper<OrganDonationRegistration, RegistrationLookupRequest>,
                RegistrationLookupRequestMapper>();

            services
                .AddSingleton<IMapper<OrganDonationResponse<ReferenceDataResponse>, OrganDonationReferenceDataResponse>,
                    OrganDonationReferenceDataResponseMapper>();
            
            services
                .AddSingleton<IMapper<string, OrganDonation.Models.Address, Address>, OrganDonationAddressMapper>();
            
            services.AddSingleton<IMapper<string, OrganDonation.Models.Name, Name>, OrganDonationNameMapper>();
            services
                .AddSingleton<IMapper<OrganDonationRegistrationRequest, RegistrationRequest>, RegistrationRequestMapper>();
            
            services
                .AddSingleton<IMapper<OrganDonationResponse<RegistrationResponse>, OrganDonationRegistrationResponse>,
                    OrganDonationRegistrationResponseMapper>();

            services.AddSingleton<IEnumMapper<string, Decision>, OrganDonationDecisionMapper>();
            services.AddSingleton<IEnumMapper<string, ChoiceState>, OrganDonationChoiceStateMapper>();
            services.AddSingleton<IEnumMapper<string, FaithDeclaration>, OrganDonationFaithDeclarationMapper>();

            services.AddSingleton<IOrganDonationGenderMapper, OrganDonationGenderMapper>();
            services.AddSingleton<IOrganDonationDonationWishesMapper, OrganDonationDonationWishesMapper>();

            base.ConfigureServices(services, configuration);
        }
    }
}