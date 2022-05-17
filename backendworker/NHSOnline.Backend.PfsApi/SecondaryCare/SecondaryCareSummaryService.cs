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
using OperationOutcome = Hl7.Fhir.Model.OperationOutcome;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareSummaryService
    {
        private const string SecondaryCareLogPrefix = "Aggregator Secondary Care Summary API";

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

                if (aggregatorResponse.FailedToParseResponse)
                {
                    await AuditSecondaryCareResult("Failed - unable to parse response");
                    return new SecondaryCareSummaryResult.BadGateway();
                }

                if (!aggregatorResponse.HasSuccessResponse)
                {
                    if (aggregatorResponse.IsUnder16Error())
                    {
                        _logger.LogInformation("{LogPrefix} failed minimum age requirement", SecondaryCareLogPrefix);
                        return new SecondaryCareSummaryResult.FailedSecondaryCareMinimumAgeRequirement();
                    }

                    await AuditSecondaryCareResult($"Failed - response code: {aggregatorResponse.StatusCode}");
                    return new SecondaryCareSummaryResult.BadGateway();
                }

                if (aggregatorResponse.Issues.Any())
                {
                    var errorMessagesForLogging = string.Join("|", aggregatorResponse.Issues.Select(MapErrorForLogging));
                    _logger.LogError("{LogPrefix} errors found in response: {ErrorMessages}", SecondaryCareLogPrefix, errorMessagesForLogging);
                    await AuditSecondaryCareResult($"Failed - errors in response: {errorMessagesForLogging}");
                    return new SecondaryCareSummaryResult.BadGateway();
                }

                var summaryResponse = _mapper.Map(aggregatorResponse.Body);

                if (summaryResponse is null)
                {
                    _logger.LogError("{LogPrefix} unsuccessfully mapped {Bundle} to {Response}. See previous log entries for more detail", SecondaryCareLogPrefix, nameof(Bundle), nameof(SummaryResponse));
                    await AuditSecondaryCareResult("Failed - mapping failed for at least one entry");
                    return new SecondaryCareSummaryResult.BadGateway();
                }

                return new SecondaryCareSummaryResult.Success(summaryResponse);
            }
            catch (NhsTimeoutException e)
            {
                _logger.LogError(e, "{LogPrefix} timed out", SecondaryCareLogPrefix);
                await AuditSecondaryCareResult("Failed - request timed out");
                return new SecondaryCareSummaryResult.Timeout();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "{LogPrefix} encountered an error", SecondaryCareLogPrefix);
                await AuditSecondaryCareResult("Failed - HTTP error");
                return new SecondaryCareSummaryResult.BadGateway();
            }
        }

        private async System.Threading.Tasks.Task AuditSecondaryCareResult(string auditText)
        {
            await _auditor.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryResult, auditText);
        }

        private string MapErrorForLogging(OperationOutcome.IssueComponent issueComponent) =>
            $"Reason: {issueComponent.Diagnostics}, " +
            $"Provider: {issueComponent.Extension.FirstOrDefault()?.Value}";
    }
}