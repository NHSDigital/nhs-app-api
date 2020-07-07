using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class P9UserSessionCreator
    {
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IAuditor _auditor;
        private readonly ILogger<UserSessionManager> _logger;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IGpSessionManager _gpSessionManager;
        private readonly IIm1CacheService _im1CacheService;

        public P9UserSessionCreator(
            ISessionCacheService sessionCacheService,
            IAuditor auditor,
            ILogger<UserSessionManager> logger,
            IGpSystemFactory gpSystemFactory,
            IGpSessionManager gpSessionManager,
            IIm1CacheService im1CacheService)
        {
            _sessionCacheService = sessionCacheService;
            _auditor = auditor;
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _gpSessionManager = gpSessionManager;
            _im1CacheService = im1CacheService;
        }

        public async Task<CreateUserSessionResult> Create(
            CitizenIdSessionResult citizenIdSessionResult,
            ServiceJourneyRulesResponse serviceJourneyRules,
            string csrfToken)
        {
            return await _auditor.Audit()
                .AccessToken(citizenIdSessionResult.Session.AccessToken)
                .NhsNumber(citizenIdSessionResult.NhsNumber)
                .Supplier(serviceJourneyRules.Journeys.Supplier)
                .Operation(AuditingOperations.GpSessionCreate)
                .Details("Attempting to create Session")
                .Execute(async () => await CreateP9UserSession(citizenIdSessionResult, serviceJourneyRules.Journeys.Supplier, csrfToken));
        }

        private async Task<CreateUserSessionResult> CreateP9UserSession(
            CitizenIdSessionResult citizenIdSessionResult,
            Supplier supplier,
            string csrfToken)
        {
            var gpSystem = FetchGpSystem(citizenIdSessionResult, supplier);
            if (gpSystem.ProcessFinishedEarly(out var gpSystemResult))
            {
                return gpSystemResult;
            }

            var gpSessionCreateResult = await CreateGpSession(citizenIdSessionResult, gpSystem.Result);
            if (gpSessionCreateResult.ProcessFinishedEarly(out var gpSessionResult))
            {
                if (gpSessionResult is CreateUserSessionResult.Failure gpSessionResultFailure
                    && gpSessionResultFailure.ErrorType is ErrorTypes.GPSessionUnavailable)
                {
                    var userSessionNoGp = await CreateP9UserSessionNoGpSystem(citizenIdSessionResult, csrfToken);
                    await DeleteConnectionTokenFromCache(citizenIdSessionResult.Im1ConnectionToken);
                    return CreateUserSessionResult.SucceededNoGpSession(userSessionNoGp);
                }

                return gpSessionResult;
            }

            var userSession = await CreateP9UserSession(citizenIdSessionResult, gpSessionCreateResult.Result, csrfToken);
            await DeleteConnectionTokenFromCache(citizenIdSessionResult.Im1ConnectionToken);
            return CreateUserSessionResult.Succeeded(userSession);
        }

        private ProcessResult<IGpSystem, CreateUserSessionResult> FetchGpSystem(
            CitizenIdSessionResult citizenIdSessionResult,
            Supplier supplier)
        {
            if (supplier == Supplier.Unknown)
            {
                return FinalResult<IGpSystem>(
                    $"Failed to determine the GP system based on ODS code '{citizenIdSessionResult.Session.OdsCode}'",
                    new ErrorTypes.LoginOdsCodeNotFoundOrNotSupported());
            }
            var gpSystem = _gpSystemFactory.CreateGpSystem(supplier);
                _logger.LogDebug($"Fetch GP System: '{gpSystem.Supplier}'.");
                return ProcessResult.StepResult<IGpSystem, CreateUserSessionResult>(gpSystem);
        }

        private async Task<ProcessResult<GpUserSession, CreateUserSessionResult>> CreateGpSession(
            CitizenIdSessionResult citizenIdSessionResult, IGpSystem gpSystem)
        {
            var gpSessionCreateResult = await _gpSessionManager.CreateSession(new GpSessionCreateArgs(gpSystem, citizenIdSessionResult));
            return gpSessionCreateResult.Accept(new GpSessionCreateResultVisitor(_logger, gpSystem.Supplier));
        }

        private async Task<P9UserSession> CreateP9UserSession(CitizenIdSessionResult citizenIdSessionResult,
            GpUserSession gpUserSession, string csrfToken)
        {
            var userSession = new P9UserSession(
                csrfToken,
                citizenIdSessionResult.NhsNumber,
                citizenIdSessionResult.Session,
                gpUserSession,
                citizenIdSessionResult.Im1ConnectionToken);

            var sessionId = await _sessionCacheService.CreateUserSession(userSession);
            _logger.LogDebug($"Created Session Id: '{sessionId}'");
            return userSession;
        }

        private async Task<P9UserSession> CreateP9UserSessionNoGpSystem(CitizenIdSessionResult citizenIdSessionResult,
            string csrfToken)
        {
            _logger.LogInformation("P9 user session created with no GP session");
            var userSession = new P9UserSession(
                csrfToken,
                citizenIdSessionResult.Session,
                citizenIdSessionResult.NhsNumber,
                citizenIdSessionResult.Im1ConnectionToken);

            var sessionId = await _sessionCacheService.CreateUserSession(userSession);
            _logger.LogDebug($"Created Session Id: '{sessionId}'");
            return userSession;
        }

        private async Task DeleteConnectionTokenFromCache(string im1ConnectionToken)
        {
            if (Guid.TryParse(im1ConnectionToken, out _))
            {
                return;
            }

            var tokenObject = JObject.Parse(im1ConnectionToken);

            if (tokenObject.TryGetValue(Im1CacheService.Im1ConnectionTokenCacheKeyPropertyName,
                StringComparison.Ordinal,
                out var cacheKeyToken))
            {
                var cacheKey = cacheKeyToken?.ToString();
                if (!string.IsNullOrEmpty(cacheKey))
                {
                    await _im1CacheService.DeleteIm1ConnectionToken(cacheKey);
                }
            }
        }

        private ProcessResult<TStepResult, CreateUserSessionResult> FinalResult<TStepResult>(string message, ErrorTypes errorTypes)
        {
            _logger.LogError(message);
            return ProcessResult.FinalResult<TStepResult, CreateUserSessionResult>(CreateUserSessionResult.Failed(errorTypes, message));
        }

        private sealed class GpSessionCreateResultVisitor : IGpSessionCreateResultVisitor<ProcessResult<GpUserSession, CreateUserSessionResult>>
        {
            private readonly ILogger _logger;
            private readonly Supplier _supplier;

            public GpSessionCreateResultVisitor(ILogger logger, Supplier supplier)
            {
                _logger = logger;
                _supplier = supplier;
            }

            public ProcessResult<GpUserSession, CreateUserSessionResult> Visit(GpSessionCreateResult.Success success)
                => ProcessResult.StepResult<GpUserSession, CreateUserSessionResult>(success.UserSession);

            public ProcessResult<GpUserSession, CreateUserSessionResult> Visit(GpSessionCreateResult.Forbidden result)
                => Failed(new ErrorTypes.LoginForbidden(), result.Message);

            public ProcessResult<GpUserSession, CreateUserSessionResult> Visit(GpSessionCreateResult.BadGateway result)
                => Failed(new ErrorTypes.GPSessionUnavailable(), result.Message);

            public ProcessResult<GpUserSession, CreateUserSessionResult> Visit(GpSessionCreateResult.ErrorExceptionResult result)
                => Failed(new ErrorTypes.GPSessionUnavailable(), result.ErrorMessage);

            public ProcessResult<GpUserSession, CreateUserSessionResult> Visit(GpSessionCreateResult.Timeout result)
                => Failed(new ErrorTypes.GPSessionUnavailable(), result.ErrorMessage);

            public ProcessResult<GpUserSession, CreateUserSessionResult> Visit(GpSessionCreateResult.Unparseable result)
                => Failed(new ErrorTypes.GPSessionUnavailable(), result.ErrorMessage);

            public ProcessResult<GpUserSession, CreateUserSessionResult> Visit(GpSessionCreateResult.InternalServerError result)
                => Failed(new ErrorTypes.GPSessionUnavailable(), result.Message);

            public ProcessResult<GpUserSession, CreateUserSessionResult> Visit(GpSessionCreateResult.BadRequest result)
                => Failed(new ErrorTypes.GPSessionUnavailable(), result.Message);

            public ProcessResult<GpUserSession, CreateUserSessionResult> Visit(GpSessionCreateResult.InvalidConnectionToken result)
                => Failed(new ErrorTypes.LoginForbidden(), "Invalid connection token");

            private ProcessResult<GpUserSession, CreateUserSessionResult> Failed(ErrorTypes errorType, string message)
            {
                string errorMessage = $"Creating the session failed: {message}";

                _logger.LogError(errorMessage);
                var result = CreateUserSessionResult.Failed(errorType, errorMessage);

                return ProcessResult.FinalResult<GpUserSession, CreateUserSessionResult>(result);
            }
        }
    }
}