using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.PfsApi.NHSApim;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareSummaryService
    {
        private readonly ISecondaryCareClient _secondaryCareClient;
        private readonly ILogger<SecondaryCareSummaryService> _logger;
        private readonly INhsApimClient _nhsApimClient;

        public SecondaryCareSummaryService(
            ISecondaryCareClient secondaryCareClient,
            ILogger<SecondaryCareSummaryService> logger,
            INhsApimClient nhsApimClient)
        {
            _secondaryCareClient = secondaryCareClient;
            _logger = logger;
            _nhsApimClient = nhsApimClient;
        }

        public async Task<SecondaryCareSummaryResult> GetSummary(P9UserSession userSession)
        {
            try
            {
                var authResponse = await _nhsApimClient.GetAuthToken();

                var summaryResponse = await _secondaryCareClient.GetSummary(userSession, authResponse.Body.AccessToken);

                if (!summaryResponse.HasSuccessResponse)
                {
                    return new SecondaryCareSummaryResult.BadGateway();
                }

                if (summaryResponse.Body.Referrals?.Count() > 1)
                {
                    summaryResponse.Body.Referrals =
                        summaryResponse.Body.Referrals.OrderBy(referral => referral.ReferredDateTime);
                }

                if (summaryResponse.Body.UpcomingAppointments?.Count() > 1)
                {
                    summaryResponse.Body.UpcomingAppointments =
                        summaryResponse.Body.UpcomingAppointments.OrderBy(appointment => appointment.AppointmentDateTime);
                }

                return new SecondaryCareSummaryResult.Success(summaryResponse.Body);

            }
            catch (NhsTimeoutException e)
            {
                _logger.LogError(e.Message);
                return new SecondaryCareSummaryResult.Timeout();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e.Message);
                return new SecondaryCareSummaryResult.BadGateway();
            }
        }
    }
}