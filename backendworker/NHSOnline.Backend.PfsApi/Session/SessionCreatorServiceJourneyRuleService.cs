using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class SessionCreatorServiceJourneyRuleService
    {
        private readonly ILogger _logger;
        private readonly IServiceJourneyRulesService _serviceJourneyRules;

        public SessionCreatorServiceJourneyRuleService(
            ILogger<SessionCreator> logger,
            IServiceJourneyRulesService serviceJourneyRules)
        {
            _logger = logger;
            _serviceJourneyRules = serviceJourneyRules;
        }

        internal async Task<ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult>> Fetch(
            CitizenIdSessionResult citizenIdSessionResult)
        {
            var odsCode = citizenIdSessionResult.Session.OdsCode;

            _logger.LogInformation($"Retrieving Service Journey Rules for ods code: {odsCode}");
            var serviceJourneyRulesConfig = await _serviceJourneyRules.GetServiceJourneyRulesForOdsCode(odsCode);

            return serviceJourneyRulesConfig.Accept(new ServiceJourneyRulesConfigResultVisitor(_logger));
        }

        private sealed class ServiceJourneyRulesConfigResultVisitor
            : IServiceJourneyRulesConfigResultVisitor<ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult>>
        {
            private readonly ILogger _logger;

            public ServiceJourneyRulesConfigResultVisitor(ILogger logger) => _logger = logger;

            public ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult> Visit(ServiceJourneyRulesConfigResult.Success result)
                => StepResult(result.Response);

            public ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult> Visit(ServiceJourneyRulesConfigResult.NotFound result)
                => ErrorResult(new ErrorTypes.LoginOdsCodeNotFoundOrNotSupported());

            public ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult> Visit(ServiceJourneyRulesConfigResult.InternalServerError result)
                => ErrorResult(new ErrorTypes.LoginServiceJourneyRulesOtherError());

            private ProcessResult<ServiceJourneyRulesResponse, CreateSessionResult> ErrorResult(ErrorTypes errorTypes)
            {
                var errorMessage = $"Retrieving Service Journey Rules failed with status code: '{errorTypes.StatusCode}'";
                _logger.LogError(errorMessage);

                return FinalResult<ServiceJourneyRulesResponse>(errorTypes);
            }

            private static ProcessResult<TStepResult, CreateSessionResult> StepResult<TStepResult>(TStepResult result)
                => ProcessResult.StepResult<TStepResult, CreateSessionResult>(result);

            private static ProcessResult<TFinalResult, CreateSessionResult> FinalResult<TFinalResult>(ErrorTypes errorTypes)
                => ProcessResult.FinalResult<TFinalResult, CreateSessionResult>(new CreateSessionResult.ErrorResult(errorTypes));
        }
    }
}