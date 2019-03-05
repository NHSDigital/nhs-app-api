using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    internal class OrganDonationUpdateService 
    {
        private readonly ILogger<OrganDonationUpdateService> _logger;

        private readonly IMapper<OrganDonationRegistrationRequest, RegistrationRequest>
            _registrationRequestMapper;

        private readonly IMapper<OrganDonationResponse<RegistrationResponse>, OrganDonationRegistrationResponse>
            _registrationResponseMapper;

        private readonly IOrganDonationClient _organDonationClient;

        public OrganDonationUpdateService(
            ILogger<OrganDonationUpdateService> logger,
            IMapper<OrganDonationRegistrationRequest, RegistrationRequest> registrationRequestMapper,
            IMapper<OrganDonationResponse<RegistrationResponse>, OrganDonationRegistrationResponse>
                registrationResponseMapper,
            IOrganDonationClient organDonationClient)
        {
            _logger = logger;
            _organDonationClient = organDonationClient;
            _registrationRequestMapper = registrationRequestMapper;
            _registrationResponseMapper = registrationResponseMapper;
        }


        public async Task<OrganDonationRegistrationResult> Update(
            OrganDonationRegistrationRequest request,
            UserSession userSession)
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

                return HandleErrorCodes(clientResponse.StatusCode);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request to register an update to organ donation decision");
                return new OrganDonationRegistrationResult.SystemError();
            }
        }

        private OrganDonationRegistrationResult HandleErrorCodes(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.RequestTimeout:
                    _logger.LogDebug("The organ donation registration timed-out");
                    return new OrganDonationRegistrationResult.Timeout();
                default:
                    _logger.LogDebug("Something went wrong when registering organ donation decision");
                    return new OrganDonationRegistrationResult.UpstreamError();
            }
        }
    }
}