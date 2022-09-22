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
    public class SecondaryCareWaitTimesService
    {
        private const string SecondaryCareLogPrefix = "Aggregator Secondary Care WaitTimes API";

        private readonly ILogger<SecondaryCareWaitTimesService> _logger;
        private readonly ISecondaryCareWaitTimesMapper _mapper;
        private readonly ISecondaryCareApimService _secondaryCareApimService;
        private readonly ISecondaryCareClient _secondaryCareClient;
        private readonly IAuditor _auditor;
        private readonly ISecondaryCareConfig _config;

        public SecondaryCareWaitTimesService(
            ILogger<SecondaryCareWaitTimesService> logger,
            ISecondaryCareWaitTimesMapper mapper,
            ISecondaryCareApimService secondaryCareApimService,
            ISecondaryCareClient secondaryCareClient,
            IAuditor auditor,
            ISecondaryCareConfig config)
        {
            _logger = logger;
            _mapper = mapper;
            _secondaryCareApimService = secondaryCareApimService;
            _secondaryCareClient = secondaryCareClient;
            _auditor = auditor;
            _config = config;
        }

        public async Task<SecondaryCareWaitTimesResult> GetWaitTimes(P9UserSession userSession)
        {
            try
            {
                if (!_config.WaitTimesEnabled)
                {
                    return new SecondaryCareWaitTimesResult.NotEnabled();
                }

                var authResponseResult = await _secondaryCareApimService.GetAuthToken(userSession,AuditingOperations.SecondaryCareGetWaitTimesRequest);

                if (!authResponseResult.isSuccess)
                {
                    return new SecondaryCareWaitTimesResult.BadGateway();
                }

                var aggregatorResponse =
                    await _secondaryCareClient.GetResponse(userSession, authResponseResult.authToken.Body.AccessToken, _config.WaitTimesPath);

                if (aggregatorResponse.FailedToParseResponse)
                {
                    await AuditSecondaryCareResult("Failed - unable to parse response");
                    return new SecondaryCareWaitTimesResult.BadGateway();
                }

                if (!aggregatorResponse.HasSuccessResponse)
                {
                    await AuditSecondaryCareResult($"Failed - response code: {aggregatorResponse.StatusCode}");
                    return new SecondaryCareWaitTimesResult.BadGateway();
                }

                if (aggregatorResponse.Issues.Any())
                {
                    var errorMessagesForLogging = string.Join("|", aggregatorResponse.Issues.Select(MapErrorForLogging));
                    _logger.LogError("{LogPrefix} errors found in response: {ErrorMessages}", SecondaryCareLogPrefix, errorMessagesForLogging);
                    await AuditSecondaryCareResult($"Failed - errors in response: {errorMessagesForLogging}");
                }

                var summaryResponse = _mapper.Map(aggregatorResponse.Body);

                if (summaryResponse is null)
                {
                    _logger.LogError("{LogPrefix} unsuccessfully mapped {Bundle} to {WaitTimesResponse}. See previous log entries for more detail", SecondaryCareLogPrefix, nameof(Bundle), nameof(WaitTimesResponse));
                    await AuditSecondaryCareResult("Failed - mapping failed for at least one entry");
                    return new SecondaryCareWaitTimesResult.BadGateway();
                }

                return new SecondaryCareWaitTimesResult.Success(summaryResponse);
            }
            catch (NhsTimeoutException e)
            {
                _logger.LogError(e, "{LogPrefix} timed out", SecondaryCareLogPrefix);
                await AuditSecondaryCareResult("Failed - request timed out");
                return new SecondaryCareWaitTimesResult.Timeout();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "{LogPrefix} encountered an error", SecondaryCareLogPrefix);
                await AuditSecondaryCareResult("Failed - HTTP error");
                return new SecondaryCareWaitTimesResult.BadGateway();
            }
        }

        private async System.Threading.Tasks.Task AuditSecondaryCareResult(string auditText)
        {
            await _auditor.PostOperationAudit(AuditingOperations.SecondaryCareGetWaitTimesResult, auditText);
        }

        private string MapErrorForLogging(OperationOutcome.IssueComponent issueComponent) =>
            $"Reason: {issueComponent.Diagnostics}, " +
            $"Provider: {issueComponent.Extension.FirstOrDefault()?.Value}";
    }
}