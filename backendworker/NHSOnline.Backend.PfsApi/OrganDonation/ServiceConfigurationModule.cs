using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.PfsApi.OrganDonation.Mappers;
using NHSOnline.Backend.Support.Session;
using Address = NHSOnline.Backend.PfsApi.OrganDonation.ApiModels.Address;
using Name = NHSOnline.Backend.PfsApi.OrganDonation.ApiModels.Name;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<OrganDonationHttpRequestIdentifier>();
            services.AddSingleton<IOrganDonationConfig, OrganDonationConfig>();

            services.AddSingleton<IOrganDonationClient, OrganDonationClient>();
            services.AddSingleton<OrganDonationHttpClientHandler>();

            services.AddHttpClient<OrganDonationHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<OrganDonationHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<OrganDonationHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<OrganDonationHttpRequestIdentifier>>();

            services.AddSingleton<IOrganDonationValidationService, OrganDonationValidationService>();
            services.AddSingleton<IOrganDonationService, OrganDonationService>();
            services.AddSingleton<OrganDonationLookupService>();
            services.AddSingleton<OrganDonationReferenceDataService>();
            services.AddSingleton<OrganDonationRegistrationService>();
            services.AddSingleton<OrganDonationUpdateService>();
            services.AddSingleton<OrganDonationWithdrawService>();

            services.AddSingleton<IOrganDonationDataMaps, OrganDonationDataMaps>();

            services.AddSingleton<IMapper<DemographicsResponse, P9UserSession, OrganDonationRegistration>,
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
                .AddSingleton<IMapper<string, Models.Address, Address>, OrganDonationAddressMapper>();

            services.AddSingleton<IMapper<Models.Name, Name>, OrganDonationNameMapper>();
            services.AddSingleton<IMapper<OrganDonationRegistrationRequest, RegistrationRequest>,
                RegistrationRequestMapper>();

            services.AddSingleton<IMapper<OrganDonationWithdrawRequest, WithdrawRequest>,
                    WithdrawRequestMapper>();

            services
                .AddSingleton<IMapper<OrganDonationResponse<OrganDonationBasicResponse>, OrganDonationRegistrationResponse>,
                    OrganDonationRegistrationResponseMapper>();

            services.AddSingleton<IEnumMapper<string, Decision>, OrganDonationDecisionMapper>();
            services.AddSingleton<IEnumMapper<string, ChoiceState>, OrganDonationChoiceStateMapper>();
            services.AddSingleton<IEnumMapper<string, FaithDeclaration>, OrganDonationFaithDeclarationMapper>();

            services.AddSingleton<IOrganDonationGenderMapper, OrganDonationGenderMapper>();
            services.AddSingleton<IOrganDonationDonationWishesMapper, OrganDonationDonationWishesMapper>();
            services.AddSingleton<IOrganDonationIdentifierMapper, OrganDonationIdentifierMapper>();

            services.AddSingleton<IMapper<HttpStatusCode, OrganDonationResult>, OrganDonationResultErrorMapper>();
            services
                .AddSingleton<IMapper<HttpStatusCode, OrganDonationRegistrationResult>,
                    OrganDonationRegistrationResultErrorMapper>();
            services
                .AddSingleton<IMapper<HttpStatusCode, OrganDonationReferenceDataResult>,
                    OrganDonationReferenceDataResultErrorMapper>();
            services
                .AddSingleton<IMapper<HttpStatusCode, OrganDonationWithdrawResult>,
                    OrganDonationWithdrawResultErrorMapper>();

            base.ConfigureServices(services, configuration);
        }
    }
}