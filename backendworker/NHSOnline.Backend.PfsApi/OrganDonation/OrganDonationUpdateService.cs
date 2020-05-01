using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    internal class OrganDonationUpdateService 
    {
        private readonly ILogger<OrganDonationUpdateService> _logger;

        private readonly IMapper<OrganDonationRegistrationRequest, RegistrationRequest>
            _registrationRequestMapper;

        private readonly IMapper<OrganDonationResponse<OrganDonationBasicResponse>, OrganDonationRegistrationResponse>
            _registrationResponseMapper;
        
        private readonly IMapper<HttpStatusCode, OrganDonationRegistrationResult> _organDonationRegistrationResultErrorMapper;

        private readonly IOrganDonationClient _organDonationClient;

        public OrganDonationUpdateService(
            ILogger<OrganDonationUpdateService> logger,
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


        public async Task<OrganDonationRegistrationResult> Update(
            OrganDonationRegistrationRequest request,
            P9UserSession userSession)
        {
            try
            {
                var registrationRequest = _registrationRequestMapper.Map(request);

                _logger.LogDebug("Attempting to update organ donation decision");
                var clientResponse = await _organDonationClient.PutUpdate(registrationRequest, userSession);

                if (clientResponse.HasSuccessResponse)
                {
                    _logger.LogDebug("Update successful");
                    var response = _registrationResponseMapper.Map(clientResponse);
                    return new OrganDonationRegistrationResult.SuccessfullyRegistered(response);
                }

                return _organDonationRegistrationResultErrorMapper.Map(clientResponse.StatusCode);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request to register an update to organ donation decision");
                return new OrganDonationRegistrationResult.SystemError();
            }
        }
    }
}