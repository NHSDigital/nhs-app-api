extern alias r4;

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.SecondaryCare.Mappers;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.PfsApi.NHSApim;
using NHSOnline.Backend.Support.Session;
using r4::Hl7.Fhir.Model;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareSummaryService
    {
        private readonly ILogger<SecondaryCareSummaryService> _logger;
        private readonly ISecondaryCareSummaryMapper _mapper;
        private readonly ISecondaryCareClient _secondaryCareClient;
        private readonly INhsApimClient _nhsApimClient;

        public SecondaryCareSummaryService(
            ILogger<SecondaryCareSummaryService> logger,
            ISecondaryCareSummaryMapper mapper,
            ISecondaryCareClient secondaryCareClient,
            INhsApimClient nhsApimClient)
        {
            _logger = logger;
            _mapper = mapper;
            _secondaryCareClient = secondaryCareClient;
            _nhsApimClient = nhsApimClient;
        }

        public async Task<SecondaryCareSummaryResult> GetSummary(P9UserSession userSession)
        {
            try
            {
                var authResponse = await _nhsApimClient.GetAuthToken(userSession.CitizenIdUserSession.NhsLoginIdToken);

                var aggregatorResponse = await _secondaryCareClient.GetSummary(userSession, authResponse.Body.AccessToken);

                if (!aggregatorResponse.HasSuccessResponse || aggregatorResponse.FailedToParseResponse)
                {
                    return new SecondaryCareSummaryResult.BadGateway();
                }

                var summaryResponse = _mapper.Map(aggregatorResponse.Body);

                if (summaryResponse is null)
                {
                    _logger.LogError("Unsuccessfully mapped {CarePlan} to {Response}. See previous log entries for more detail", nameof(CarePlan), nameof(SummaryResponse));
                    return new SecondaryCareSummaryResult.BadGateway();
                }

                return new SecondaryCareSummaryResult.Success(summaryResponse);
            }
            catch (NhsTimeoutException e)
            {
                _logger.LogError(e, "Aggregator Secondary Care Summary API timed out");
                return new SecondaryCareSummaryResult.Timeout();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Aggregator Secondary Care Summary API encountered an error");
                return new SecondaryCareSummaryResult.BadGateway();
            }
        }
    }
}