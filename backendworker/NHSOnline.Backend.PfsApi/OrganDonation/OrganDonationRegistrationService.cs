using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    internal class OrganDonationRegistrationService 
    {
        private readonly ILogger<OrganDonationRegistrationService> _logger;
        private readonly IMapper<OrganDonationRegistrationRequest, RegistrationRequest>
            _registrationRequestMapper;
        private readonly IMapper<OrganDonationResponse<OrganDonationBasicResponse>, OrganDonationRegistrationResponse>
            _registrationResponseMapper;
        private readonly IMapper<HttpStatusCode, OrganDonationRegistrationResult> _organDonationRegistrationResultErrorMapper;

        private readonly IOrganDonationClient _organDonationClient;

        public OrganDonationRegistrationService(
            ILogger<OrganDonationRegistrationService> logger,
            IMapper<OrganDonationRegistrationRequest, RegistrationRequest> registrationRequestMapper,
            IMapper<OrganDonationResponse<OrganDonationBasicResponse>, OrganDonationRegistrationResponse>
                registrationResponseMapper,
            IMapper<HttpStatusCode, OrganDonationRegistrationResult> organDonationRegistrationResultErrorMapper,
            IOrganDonationClient organDonationClient)
        {
            _logger = logger;
            _organDonationClient = organDonationClient;
            _registrationRequestMapper = registrationRequestMapper;
            _registrationResponseMapper = registrationResponseMapper;
            _organDonationRegistrationResultErrorMapper = organDonationRegistrationResultErrorMapper;
        }

        public async Task<OrganDonationRegistrationResult> Register(
            OrganDonationRegistrationRequest request,
            P9UserSession userSession)
        {
            try
            {
                var registrationRequest = _registrationRequestMapper.Map(request);

                _logger.LogDebug("Attempting to register organ donation decision");
                var clientResponse = await _organDonationClient.PostRegistration(registrationRequest, userSession);

                if (!clientResponse.HasSuccessResponse)
                    return _organDonationRegistrationResultErrorMapper.Map(clientResponse.StatusCode);
                
                _logger.LogDebug("Registration successful");
                var response = _registrationResponseMapper.Map(clientResponse);
                return new OrganDonationRegistrationResult.SuccessfullyRegistered(response);

            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request to register organ donation decision");
                return new OrganDonationRegistrationResult.SystemError();
            }
        }
    }
}