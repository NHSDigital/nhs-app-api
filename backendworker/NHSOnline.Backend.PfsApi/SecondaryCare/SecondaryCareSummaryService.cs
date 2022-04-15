using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareSummaryService
    {
        private readonly ISecondaryCareClient _secondaryCareClient;
        private readonly ILogger<SecondaryCareSummaryService> _logger;

        public SecondaryCareSummaryService(
            ISecondaryCareClient secondaryCareClient,
            ILogger<SecondaryCareSummaryService> logger)
        {
            _secondaryCareClient = secondaryCareClient;
            _logger = logger;
        }

        public async Task<SecondaryCareSummaryResult> GetSummary(P9UserSession userSession)
        {
            try
            {
                var summaryResponse = await _secondaryCareClient.GetSummary(userSession);

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