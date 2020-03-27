using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    internal class TppSessionService : ISessionService
    {
        private readonly ITppClient _client;
        private readonly ITppClientRequest<TppUserSession, LogoffReply> _logoff;
        private readonly ITppClientRequest<Authenticate, AuthenticateReply> _authenticate;
        private readonly ITppClientRequest<TppUserSession, ListServiceAccessesReply> _listServiceAccesses;
        private readonly ILogger<TppSessionService> _logger;
        private readonly ITppSessionMapper _sessionMapper;

        public TppSessionService(
            ITppClient client,
            ITppClientRequest<TppUserSession, LogoffReply> logoff,
            ITppClientRequest<Authenticate, AuthenticateReply> authenticate,
            ILogger<TppSessionService> logger,
            ITppSessionMapper sessionMapper,
            ITppClientRequest<TppUserSession, ListServiceAccessesReply> listServiceAccesses)
        {
            _client = client;
            _logoff = logoff;
            _authenticate = authenticate;
            _logger = logger;
            _sessionMapper = sessionMapper;
            _listServiceAccesses = listServiceAccesses;
        }

        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            try
            {
                var authenticateReply = await AuthenticatePost(connectionToken, odsCode);

                if (!authenticateReply.HasSuccessResponse)
                {
                    return CheckFailureTypeForGpSessionCreateResult(authenticateReply);
                }

                LogProxyInformation(authenticateReply.Body);

                var userSession = _sessionMapper.Map(authenticateReply, odsCode, nhsNumber);
                if (!userSession.HasValue)
                {
                    _logger.LogError("Cannot create a valid session from Tpp response");
                    return new GpSessionCreateResult.BadGateway();
                }

                var tppUserSession = userSession.ValueOrFailure();

                // The PatientSelected call is only required if
                // more than 1 person is found in the response.
                if (tppUserSession.ProxyPatients.Any())
                {
                    await _client.PatientSelectedPost(tppUserSession);
                }

                var serviceAccess = await _listServiceAccesses.Post(tppUserSession);

                serviceAccess.Body?.ServiceAccess?.ForEach(s => {
                    if (string.Equals(s.Description,
                            Constants.TppLinkServicesAccessConstants.Im1MessagingService,
                            StringComparison.Ordinal)
                        && s.Status == Constants.TppLinkServicesAccessConstants.AvailableCode )
                    {
                        tppUserSession.Im1MessagingEnabled = true;
                        _logger.LogInformation("PFS messaging is enabled");
                    }
                });

                _logger.LogDebug($"TPP user session successfully create to OdsCode {odsCode}");
                return new GpSessionCreateResult.Success(tppUserSession);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Failed request to create TPP user session, HttpRequestException has been thrown.");
                return new GpSessionCreateResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            try
            {
                _logger.LogEnter();

                var tppUserSession = (TppUserSession) gpUserSession;
                var logoffReply = await _logoff.Post(tppUserSession);

                if (!logoffReply.HasSuccessResponse)
                {
                    return new SessionLogoffResult.BadGateway();
                }

                _logger.LogDebug($"TPP user session successfully deleted");
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
                await _client.PatientSelectedPost(tppUserSession);

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

        private void LogProxyInformation(AuthenticateReply response)
        {
            var patientId = response.User?.Person?.PatientId;

            if (string.IsNullOrWhiteSpace(patientId))
            {
                _logger.LogWarning($"TPP user with no {nameof(AuthenticateReply.User.Person.PatientId)}");
                return;
            }

            var patientAccessItems = response.Registration?.PatientAccess ?? new List<PatientAccess>();

            var selfPatient = patientAccessItems.FirstOrDefault(
                x => patientId.Equals(x.PatientId, StringComparison.Ordinal));

            var linkedPatients = patientAccessItems
                .Where(x => !patientId.Equals(x.PatientId, StringComparison.Ordinal))
                .ToList();

            if (selfPatient == null)
            {
                _logger.LogWarning(
                    $"TPP user details not found in {nameof(AuthenticateReply.Registration.PatientAccess)}");
                return;
            }

            var userPractice = new
            {
                selfPatient.SiteDetails?.UnitName,
                selfPatient.SiteDetails?.Address?.Address,
            };

            if (userPractice.UnitName == null || userPractice.Address == null)
            {
                _logger.LogWarning(
                    $"TPP user practice details not specified. unitName:{userPractice.UnitName} address:{userPractice.Address}");
                return;
            }

            var differentPracticeAddressCount = linkedPatients.Count(x =>
                !userPractice.UnitName.Equals(x.SiteDetails.UnitName, StringComparison.Ordinal) ||
                !userPractice.Address.Equals(x.SiteDetails.Address.Address, StringComparison.Ordinal));

            _logger.LogInformation(
                $"User has linked_accounts={linkedPatients.Count}, with different_ods_codes_to_user={differentPracticeAddressCount}");
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
                _logger.LogError("Failed to authenticate user for TPP - Problem logging on");
                return new GpSessionCreateResult.Forbidden();
            }

            _logger.LogError("Failed to authenticate user for TPP");
            return new GpSessionCreateResult.BadGateway();
        }
    }
}