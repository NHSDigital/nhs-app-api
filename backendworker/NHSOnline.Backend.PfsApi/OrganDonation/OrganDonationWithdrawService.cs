using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    internal class OrganDonationWithdrawService 
    {
        private readonly ILogger<OrganDonationUpdateService> _logger;
        private readonly IMapper<OrganDonationWithdrawRequest, WithdrawRequest> _withdrawRequestMapper;
        private readonly IMapper<HttpStatusCode, OrganDonationWithdrawResult> _organDonationWithdrawResultErrorMapper;

        private readonly IOrganDonationClient _organDonationClient;

        public OrganDonationWithdrawService(
            ILogger<OrganDonationUpdateService> logger,
            IMapper<OrganDonationWithdrawRequest, WithdrawRequest> withdrawRequestMapper,
            IMapper<HttpStatusCode, OrganDonationWithdrawResult> organDonationWithdrawResultErrorMapper,
            IOrganDonationClient organDonationClient)
        {
            _logger = logger;
            _withdrawRequestMapper = withdrawRequestMapper;
            _organDonationClient = organDonationClient;
            _organDonationWithdrawResultErrorMapper = organDonationWithdrawResultErrorMapper;
        }

        public async Task<OrganDonationWithdrawResult> Withdraw(OrganDonationWithdrawRequest request, UserSession userSession)
        {
            try
            {
                var registrationRequest = _withdrawRequestMapper.Map(request);

                _logger.LogDebug("Attempting to withdraw organ donation decision");
                var clientResponse = await _organDonationClient.Delete(registrationRequest, userSession);

                if (clientResponse.HasSuccessResponse)
                {
                    _logger.LogDebug("Withdraw successful");
                    return new OrganDonationWithdrawResult.SuccessfullyWithdrawn();
                }
                _logger.LogInformation("Error received from Organ Donation while withdrawing registration " +
                                       clientResponse.ErrorForLogging);
                return _organDonationWithdrawResultErrorMapper.Map(clientResponse.StatusCode);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request to withdraw an organ donation decision");
                return new OrganDonationWithdrawResult.SystemError();
            }
        }
    }
}