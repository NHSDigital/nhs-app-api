using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class SessionCreatorCitizenIdService
    {
        private readonly ILogger _logger;
        private readonly ICitizenIdSessionService _citizenIdSessionService;
        private readonly IOdsCodeMassager _odsCodeMassager;
        private readonly IAuditor _auditor;

        public SessionCreatorCitizenIdService(
            ILogger<SessionCreatorCitizenIdService> logger,
            ICitizenIdSessionService citizenIdSessionService,
            IOdsCodeMassager odsCodeMassager,
            IAuditor auditor)
        {
            _logger = logger;
            _citizenIdSessionService = citizenIdSessionService;
            _odsCodeMassager = odsCodeMassager;
            _auditor = auditor;
        }

        internal async Task<ProcessResult<CitizenIdSessionResult, CreateSessionResult>> FetchUserProfile(ICreateSessionRequest request)
        {
            var citizenIdSessionResult = await _citizenIdSessionService.Create(request.AuthCode, request.CodeVerifier, request.RedirectUrl);

            if (!citizenIdSessionResult.StatusCode.IsSuccessStatusCode())
            {
                return ErrorResult(citizenIdSessionResult);
            }

            MassageOdsCode(citizenIdSessionResult);

            await Audit(citizenIdSessionResult);

            _logger.LogInformation($"NhsNumber={citizenIdSessionResult.NhsNumber.RemoveWhiteSpace()}");

            return StepResult(citizenIdSessionResult);
        }

        private async Task Audit(CitizenIdSessionResult citizenIdSessionResult)
        {
            await _auditor.Audit()
                .AccessToken(citizenIdSessionResult.Session.AccessToken)
                .NhsNumber(citizenIdSessionResult.NhsNumber)
                .Supplier(Supplier.Unknown)
                .Operation(AuditingOperations.CitizenIdSessionCreate)
                .Details("Create Citizen Id Session")
                .Execute(() => Task.FromResult(citizenIdSessionResult));
        }

        private void MassageOdsCode(CitizenIdSessionResult citizenIdSessionResult)
            => citizenIdSessionResult.Session.OdsCode = _odsCodeMassager.CheckOdsCode(citizenIdSessionResult.Session.OdsCode);

        private ProcessResult<CitizenIdSessionResult, CreateSessionResult> ErrorResult(CitizenIdSessionResult citizenIdSessionResult)
        {
            var errorTypes = citizenIdSessionResult.StatusCode switch
            {
                // 502 Bad gateway error references differ by source API. The other error types do not.
                StatusCodes.Status502BadGateway => ErrorTypes.LookupErrorType(_logger, ErrorCategory.Login,
                    StatusCodes.Status502BadGateway, SourceApi.NhsLogin),
                _ => ErrorTypes.LookupErrorType(_logger, ErrorCategory.Login, citizenIdSessionResult.StatusCode)
            };

            return FinalResult<CitizenIdSessionResult>(errorTypes);
        }

        private static ProcessResult<TStepResult, CreateSessionResult> StepResult<TStepResult>(TStepResult result)
            => ProcessResult.StepResult<TStepResult, CreateSessionResult>(result);

        private static ProcessResult<TFinalResult, CreateSessionResult> FinalResult<TFinalResult>(ErrorTypes errorTypes)
            => ProcessResult.FinalResult<TFinalResult, CreateSessionResult>(new CreateSessionResult.ErrorResult(errorTypes));
    }
}