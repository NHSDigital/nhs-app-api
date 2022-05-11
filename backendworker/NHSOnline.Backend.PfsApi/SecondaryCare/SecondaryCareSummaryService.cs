extern alias r4;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
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
        private readonly IAuditor _auditor;

        public SecondaryCareSummaryService(
            ILogger<SecondaryCareSummaryService> logger,
            ISecondaryCareSummaryMapper mapper,
            ISecondaryCareClient secondaryCareClient,
            INhsApimClient nhsApimClient,
            IAuditor auditor)
        {
            _logger = logger;
            _mapper = mapper;
            _secondaryCareClient = secondaryCareClient;
            _nhsApimClient = nhsApimClient;
            _auditor = auditor;
        }

        public async Task<SecondaryCareSummaryResult> GetSummary(P9UserSession userSession)
        {
            try
            {
                var authResponse = await _nhsApimClient.GetAuthToken(userSession.CitizenIdUserSession.NhsLoginIdToken);

                var aggregatorResponse = await _secondaryCareClient.GetSummary(userSession, authResponse.Body.AccessToken);

                if (!aggregatorResponse.HasSuccessResponse)
                {
                    await AuditSecondaryCareResult($"Failed - response code: {aggregatorResponse.StatusCode}");
                    return new SecondaryCareSummaryResult.BadGateway();
                }

                if (aggregatorResponse.FailedToParseResponse)
                {
                    await AuditSecondaryCareResult("Failed - unable to parse response");
                    return new SecondaryCareSummaryResult.BadGateway();
                }

                var summaryResponse = _mapper.Map(aggregatorResponse.Body, out var issues);

                if (issues.Any())
                {
                    var errorMessagesForLogging = string.Join("|", issues);
                    _logger.LogError("Aggregator Secondary Care Summary API errors found in response: {ErrorMessages}", errorMessagesForLogging);
                    await AuditSecondaryCareResult($"Failed - errors in response: {errorMessagesForLogging}");
                    return new SecondaryCareSummaryResult.BadGateway();
                }

                if (summaryResponse is null)
                {
                    _logger.LogError("Aggregator Secondary Care Summary API unsuccessfully mapped {Bundle} to {Response}. See previous log entries for more detail", nameof(Bundle), nameof(SummaryResponse));
                    await AuditSecondaryCareResult("Failed - mapping failed for at least one entry");
                    return new SecondaryCareSummaryResult.BadGateway();
                }

                return new SecondaryCareSummaryResult.Success(summaryResponse);
            }
            catch (NhsTimeoutException e)
            {
                _logger.LogError(e, "Aggregator Secondary Care Summary API timed out");
                await AuditSecondaryCareResult("Failed - request timed out");
                return new SecondaryCareSummaryResult.Timeout();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Aggregator Secondary Care Summary API encountered an error");
                await AuditSecondaryCareResult("Failed - HTTP error");
                return new SecondaryCareSummaryResult.BadGateway();
            }
        }

        private async System.Threading.Tasks.Task AuditSecondaryCareResult(string auditText)
        {
            await _auditor.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryResult, auditText);
        }
    }
}