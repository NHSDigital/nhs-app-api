using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    internal class TppSessionService : ISessionService
    {
        private const string Im1MessagingService = "Messaging";
        private const string ServiceAvailableCode = "A";

        private readonly ILogger<TppSessionService> _logger;
        private readonly ITppSessionMapper _sessionMapper;
        private readonly TppTokenValidationService _tokenValidationService;
        private readonly ITppLinkedAccountsService _tppLinkedAccountsService;

        private readonly ITppClientRequest<Authenticate, AuthenticateReply> _authenticate;
        private readonly ITppClientRequest<TppRequestParameters, PatientSelectedReply> _patientSelected;
        private readonly ITppClientRequest<TppUserSession, ListServiceAccessesReply> _listServiceAccesses;
        private readonly ITppClientRequest<TppRequestParameters, LogoffReply> _logoff;

        public TppSessionService(
            ILogger<TppSessionService> logger,
            ITppSessionMapper sessionMapper,
            TppTokenValidationService tokenValidationService,
            ITppLinkedAccountsService tppLinkedAccountService,
            ITppClientRequest<Authenticate, AuthenticateReply> authenticate,
            ITppClientRequest<TppRequestParameters, PatientSelectedReply> patientSelected,
            ITppClientRequest<TppUserSession, ListServiceAccessesReply> listServiceAccesses,
            ITppClientRequest<TppRequestParameters, LogoffReply> logoff)
        {
            _logger = logger;
            _sessionMapper = sessionMapper;
            _tokenValidationService = tokenValidationService;
            _tppLinkedAccountsService = tppLinkedAccountService;
            _authenticate = authenticate;
            _patientSelected = patientSelected;
            _listServiceAccesses = listServiceAccesses;
            _logoff = logoff;
        }

        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            try
            {
                if (_tokenValidationService.IsInvalidConnectionTokenFormat(connectionToken))
                {
                    _logger.LogError("Invalid Im1 connection token");
                    return new GpSessionCreateResult.InvalidConnectionToken();
                }

                var authenticateReply = await AuthenticatePost(connectionToken, odsCode);
                if (!authenticateReply.HasSuccessResponse)
                {
                    return CheckFailureTypeForGpSessionCreateResult(authenticateReply);
                }

                var createSession = CreateSession(authenticateReply, odsCode, nhsNumber);
                if (createSession.ProcessFinishedEarly(out var createSessionFinalResult))
                {
                    return createSessionFinalResult;
                }

                var tppUserSession = createSession.Result;
                _tppLinkedAccountsService.LogMismatchingPractices(authenticateReply.Body, tppUserSession.ProxyPatients);

                await SelectPatientIfMoreThanOne(authenticateReply, tppUserSession);

                if (await IsIm1MessagingEnabled(tppUserSession))
                {
                    tppUserSession.Im1MessagingEnabled = true;
                    _logger.LogInformation("PFS messaging is enabled");
                }

                _logger.LogDebug($"TPP user session successfully create to OdsCode {odsCode}");
                return new GpSessionCreateResult.Success(tppUserSession);
            }
            catch (HttpRequestException e)
            {
                const string message =
                    "Failed request to create TPP user session, HttpRequestException has been thrown.";
                _logger.LogError(e, message);
                return new GpSessionCreateResult.BadGateway(message);
            }
            catch (NhsUnparsableException unparsableException)
            {
                _logger.LogError(unparsableException.Message);
                return new GpSessionCreateResult.Unparseable(unparsableException.Message);
            }
            catch (NhsTimeoutException timeoutException)
            {
                _logger.LogError(timeoutException, timeoutException.Message);
                return new GpSessionCreateResult.Timeout(timeoutException.Message);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<bool> IsIm1MessagingEnabled(TppUserSession tppUserSession)
        {
            var serviceAccesses = await _listServiceAccesses.Post(tppUserSession);

            return serviceAccesses.Body?.ServiceAccess?.Where(IsIm1MessageService).Any(IsEnabled) ?? false;

            static bool IsIm1MessageService(ServiceAccess serviceAccess)
                => string.Equals(serviceAccess.Description, Im1MessagingService, StringComparison.Ordinal);

            static bool IsEnabled(ServiceAccess serviceAccess)
                => string.Equals(serviceAccess.Status, ServiceAvailableCode, StringComparison.Ordinal);
        }

        private async Task SelectPatientIfMoreThanOne(TppApiObjectResponse<AuthenticateReply> authenticateReply, TppUserSession tppUserSession)
        {
            // The PatientSelected call is only required if
            // more than 1 person is found in the response.
            if (authenticateReply.Body.ExtractLinkedPatients().Any())
            {
                var tppRequestParameters = new GpLinkedAccountModel(tppUserSession).BuildTppRequestParameters(_logger);

                await _patientSelected.Post(tppRequestParameters);
            }
        }

        private ProcessResult<TppUserSession, GpSessionCreateResult> CreateSession(
            TppApiObjectResponse<AuthenticateReply> authenticateReply,
            string odsCode,
            string nhsNumber)
        {
            var validProxyPatients = _tppLinkedAccountsService.ExtractValidProxyPatients(authenticateReply.Body);

            var userSession = _sessionMapper.Map(authenticateReply, odsCode, nhsNumber, validProxyPatients.Select(x => x.PatientId));
            if (!userSession.HasValue)
            {
                const string message = "Cannot create a valid session from Tpp response";
                _logger.LogError(message);
                return ProcessResult.FinalResult<TppUserSession, GpSessionCreateResult>(new GpSessionCreateResult.BadGateway(message));
            }

            return ProcessResult.StepResult<TppUserSession, GpSessionCreateResult>(userSession.ValueOrFailure());
        }

        public async Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            try
            {
                _logger.LogEnter();

                var tppUserSession = (TppUserSession)gpUserSession;

                var authenticatedId = tppUserSession.GetCurrentlyAuthenticatedId();

                if (authenticatedId.HasValue)
                {
                    var linkedAccountModel = new GpLinkedAccountModel(tppUserSession, authenticatedId.Value);
                    var tppRequestParameters = linkedAccountModel.BuildTppRequestParameters(_logger);

                    var logoffReply = await _logoff.Post(tppRequestParameters);

                    if (!logoffReply.HasSuccessResponse)
                    {
                        return new SessionLogoffResult.BadGateway();
                    }

                    _logger.LogDebug("TPP user session successfully deleted");
                }

                return new SessionLogoffResult.Success(gpUserSession);
            }

            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Failed request to logoff TPP user session, HttpRequestException has been thrown.");
                return new SessionLogoffResult.BadGateway();
            }
            catch (UnauthorisedGpSystemHttpRequestException e)
            {
                _logger.LogWarning(e, "User does not have a valid session");
                return new SessionLogoffResult.Forbidden();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GpSessionRecreateResult> Recreate(string connectionToken, string odsCode, string nhsNumber, string patientId)
        {
            try
            {
                var authenticateReply = await AuthenticatePost(connectionToken, odsCode);

                if (!authenticateReply.HasSuccessResponse)
                {
                    _logger.LogError("Failed to re-authenticate user for TPP");
                    return new GpSessionRecreateResult.Failure();
                }

                var userSession= _sessionMapper.Map(authenticateReply, odsCode, nhsNumber, patientId);
                if (!userSession.HasValue)
                {
                    _logger.LogError("Cannot recreate a valid session from Tpp response");
                    return new GpSessionRecreateResult.Failure();
                }

                var tppUserSession = userSession.ValueOrFailure();

                var tppRequestParameters = new GpLinkedAccountModel(tppUserSession)
                    .BuildTppRequestParameters(_logger);

                await _patientSelected.Post(tppRequestParameters);

                _logger.LogDebug($"TPP user session successfully recreated for patientId {patientId}");
                return new GpSessionRecreateResult.Success(tppUserSession.Suid);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Failed request to recreate TPP user session, HttpRequestException has been thrown.");
                return new GpSessionRecreateResult.Failure();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<TppApiObjectResponse<AuthenticateReply>> AuthenticatePost(string connectionToken, string odsCode)
        {
            _logger.LogEnter();
            _logger.LogDebug($"Creating using ODS code: {odsCode}");

            var tppToken = connectionToken.DeserializeJson<TppConnectionToken>();
            var authenticate = new Authenticate
            {
                AccountId = tppToken.AccountId,
                Passphrase = tppToken.Passphrase,
                ProviderId = tppToken.ProviderId,
                UnitId = odsCode
            };

            return await _authenticate.Post(authenticate);
        }
        private GpSessionCreateResult CheckFailureTypeForGpSessionCreateResult(TppApiResponse authenticateReply)
        {
            if (authenticateReply.HasErrorWithCode(TppApiErrorCodes.ProblemLoggingOn))
            {
                const string message = "Failed to authenticate user for TPP - Problem logging on";
                _logger.LogError(message);
                return new GpSessionCreateResult.Forbidden(message);
            }
            else
            {
                const string message = "Failed to authenticate user for TPP";
                _logger.LogError(message);
                return new GpSessionCreateResult.BadGateway(message);
            }
        }
    }
}