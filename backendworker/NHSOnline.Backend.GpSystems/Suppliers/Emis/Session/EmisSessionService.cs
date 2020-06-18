using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Session
{
    public interface IEmisSessionService : ISessionService
    {
        Task<SessionsEndUserSessionPostResponse> SendSessionsEndUserSessionPost();
    }

    public class EmisSessionService : IEmisSessionService
    {
        private readonly IEmisClient _emisClient;
        private readonly ILogger<EmisSessionService> _logger;
        private readonly IEmisEnumMapper _emisEnumMapper;
        private readonly EmisTokenValidationService _tokenValidationService;

        private const string StandardErrorMessage = "Failed request to create Emis user session";

        private static readonly HttpStatusCode[] InvalidTokenStatusCodes =
            { HttpStatusCode.Forbidden, HttpStatusCode.BadRequest };

        public EmisSessionService(
            IEmisClient emisClient,
            ILogger<EmisSessionService> logger,
            IEmisEnumMapper emisEnumMapper,
            EmisTokenValidationService tokenValidationService)
        {
            _emisClient = emisClient;
            _logger = logger;
            _emisEnumMapper = emisEnumMapper;
            _tokenValidationService = tokenValidationService;
        }

        public async Task<SessionsEndUserSessionPostResponse> SendSessionsEndUserSessionPost()
        {
            var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
            if (!endUserSessionResponse.HasSuccessResponse)
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(endUserSessionResponse);
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.BadGateway("Call to EMIS returned an error response"));
            }

            var responseBody = endUserSessionResponse.Body;

            if (string.IsNullOrEmpty(responseBody.EndUserSessionId))
            {
                const string gpSystemDidNotProvideEndUserSessionId = "Gp system did not provide end user session Id";
                _logger.LogError(gpSystemDidNotProvideEndUserSessionId);
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.BadGateway(gpSystemDidNotProvideEndUserSessionId));
            }

            return responseBody;
        }

        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            try
            {
                _logger.LogEnter();

                if (_tokenValidationService.IsInvalidConnectionTokenFormat(connectionToken))
                {
                    _logger.LogError("Invalid Im1 connection token");
                    return new GpSessionCreateResult.InvalidConnectionToken();
                }

                var endUserSessionResponse = await SendSessionsEndUserSessionPost();

                var session = new EmisUserSession
                {
                    Id = Guid.NewGuid(),
                    EndUserSessionId = endUserSessionResponse.EndUserSessionId,
                    NhsNumber = nhsNumber,
                    OdsCode = odsCode,
                    AppointmentBookingReasonNecessity = Necessity.Mandatory,
                    PrescriptionSpecialRequestNecessity = Necessity.Optional
                };

                var emisRequestParameters = new EmisRequestParameters(session);

                var either = EmisConnectionTokenParser.Parse(connectionToken);
                var accessIdentityGuid = either.Match(guid => guid, ct => ct.AccessIdentityGuid);

                var sessionRequestTask =
                    SendSessionsRequest(endUserSessionResponse.EndUserSessionId, accessIdentityGuid, odsCode);
                var practiceSettingsTask = _emisClient.PracticeSettingsGet(emisRequestParameters, odsCode);
                await Task.WhenAll(sessionRequestTask, practiceSettingsTask);

                var patientName = await UpdateSessionWithUserSessionsResponse(session, sessionRequestTask);
                if (patientName.Failed(out var patientNameFailure))
                {
                    return patientNameFailure;
                }

                await UpdateSessionWithPracticeSettings(session, practiceSettingsTask, patientName);

                return new GpSessionCreateResult.Success(session);
            }
            catch (EmisSessionResponseErrorException responseError)
            {
                _logger.LogError(responseError,
                    $"{StandardErrorMessage}, {nameof(EmisSessionResponseErrorException)} has been thrown");
                return responseError.ErrorResult;
            }
            catch (NhsUnparsableException unparsableException)
            {
                _logger.LogError(unparsableException.Message);
                return new GpSessionCreateResult.Unparseable(unparsableException.Message);
            }
            catch (NhsTimeoutException timeoutException)
            {
                _logger.LogError(timeoutException.Message);
                return new GpSessionCreateResult.Timeout(timeoutException.Message);
            }
            catch (HttpRequestException e)
            {
                var message = $"{StandardErrorMessage}, HttpRequestException has been thrown.";
                _logger.LogError(e, message);
                return new GpSessionCreateResult.BadGateway(message);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<EmisApiObjectResponse<SessionsPostResponse>> SendSessionsRequest(string endUserSessionId, string accessIdentityGuid, string odsCode)
        {
            var sessionPostRequestModel = new SessionsPostRequest
            {
                AccessIdentityGuid = accessIdentityGuid,
                NationalPracticeCode = odsCode
            };

            var sessionsResponse = await _emisClient.SessionsPost(endUserSessionId, sessionPostRequestModel);
            if (!sessionsResponse.HasSuccessResponse)
            {
                if (InvalidTokenStatusCodes.Contains(sessionsResponse.StatusCode))
                {
                    _logger.LogEmisResponseIsForbidden();
                    _logger.LogEmisErrorResponse(sessionsResponse);
                    throw new EmisSessionResponseErrorException(new GpSessionCreateResult.Forbidden("Call to EMIS returned a forbidden response"));
                }

                _logger.LogEmisUnknownError(sessionsResponse);
                _logger.LogEmisErrorResponse(sessionsResponse);
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.BadGateway("Call to EMIS returned an error response"));
            }

            var responseBody = sessionsResponse.Body;
            if (!IsSessionsPostResponseValid(responseBody))
            {
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.BadGateway("Call to EMIS returned an invalid response"));
            }

            return sessionsResponse;
        }

        private async Task<ProcessResult<string, GpSessionCreateResult>> UpdateSessionWithUserSessionsResponse(
            EmisUserSession session,
            Task<EmisApiObjectResponse<SessionsPostResponse>> sessionRequestTask)
        {
            string patientName;

            try
            {
                var sessionResponse = await sessionRequestTask;

                session.SessionId = sessionResponse.Body.SessionId;
                session.UserPatientLinkToken = sessionResponse.Body.ExtractUserPatientLinkToken();
                session.ProxyPatients = sessionResponse.Body.ExtractLinkedPatients()
                    .Select(x => new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        OdsCode = x.NationalPracticeCode,
                        UserPatientLinkToken = x.UserPatientLinkToken,
                    })
                    .ToList();

                patientName = FormatName(sessionResponse.Body);

                LogProxyInformation(session, sessionResponse.Body);

                if (string.IsNullOrWhiteSpace(patientName))
                {
                    const string message = "No patient name found";
                    _logger.LogError(message);
                    return new GpSessionCreateResult.BadGateway(message);
                }

                if (string.IsNullOrWhiteSpace(session.UserPatientLinkToken))
                {
                    const string message = "No EMIS userPatientLinkToken found";
                    _logger.LogError(message);
                    _logger.LogEmisErrorResponse(sessionResponse);
                    return new GpSessionCreateResult.Forbidden(message);
                }
            }
            catch (EmisSessionResponseErrorException responseError)
            {
                _logger.LogError(responseError,
                    $"{StandardErrorMessage},{nameof(EmisSessionResponseErrorException)} has been thrown");
                return responseError.ErrorResult;
            }
            catch (HttpRequestException e)
            {
                var message = $"{StandardErrorMessage}, HttpRequestException has been thrown.";
                _logger.LogError(e, message);
                var finalResult = new GpSessionCreateResult.BadGateway(message);
                return finalResult;
            }

            return patientName;
        }

        private async Task UpdateSessionWithPracticeSettings(
            EmisUserSession session,
            Task<EmisApiObjectResponse<PracticeSettingsGetResponse>> practiceSettingsTask,
            string patientName)
        {
            try
            {
                var practiceResponse = await practiceSettingsTask;

                session.AppointmentBookingReasonNecessity =
                    _emisEnumMapper.MapNecessity(practiceResponse?.Body?.InputRequirements?.AppointmentBookingReason,
                        Necessity.Mandatory);

                session.PrescriptionSpecialRequestNecessity =
                    _emisEnumMapper.MapNecessity(practiceResponse?.Body?.InputRequirements?.PrescribingComment,
                        Necessity.Optional);

                session.Im1MessagingEnabled =
                    practiceResponse?.Body?.Services?.PracticePatientCommunicationSupported ?? false;

                session.Name = patientName;

                LogAppointmentsAndPrescriptionsNecessityValues(session);

                _logger.LogInformation(
                    $"Enabled services for practice {session.OdsCode}: {practiceResponse?.Body?.Services}");
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e,
                    "Failed request to retrieve practice settings, HttpRequestException has been thrown.");
            }
        }

        private void LogAppointmentsAndPrescriptionsNecessityValues(EmisUserSession session)
        {
            var kvp = new Dictionary<string, string>
            {
                { "Appointments Booking Reason Necessity Value", $"{session.AppointmentBookingReasonNecessity} " },
                { "Prescriptions Special Request Necessity Value",  $"{session.PrescriptionSpecialRequestNecessity}" }
            };

            _logger.LogInformationKeyValuePairs("Necessity Values for User Session", kvp);
        }

        private void LogProxyInformation(EmisUserSession emisUserSession, SessionsPostResponse sessionsPostResponse)
        {
            var userPatientLinks = sessionsPostResponse.ExtractLinkedPatients().ToList();
            var selfPatient = sessionsPostResponse.ExtractSelfPatient();
            var userOdsCode = selfPatient != null ? selfPatient.NationalPracticeCode : "";
            var differentOdsCode = userPatientLinks.Count(x => !x.NationalPracticeCode.Equals(userOdsCode, StringComparison.Ordinal));

            _logger.LogInformation(
                $"User has linked_accounts={userPatientLinks.Count}, with different_ods_codes_to_user={differentOdsCode}");

            foreach (var proxyPatient in emisUserSession.ProxyPatients)
            {
                if (!string.Equals(emisUserSession.OdsCode,proxyPatient.OdsCode, StringComparison.Ordinal))
                {
                    _logger.LogInformation(
                        $"Proxy Patient with Guid {proxyPatient.Id} has different " +
                        $"OdsCode {proxyPatient.OdsCode} from main user {emisUserSession.OdsCode}");
                }
            }
        }

        private static string FormatName(SessionsPostResponse sessionResponse)
        {
            return string.Join(" ", new[] { sessionResponse.Title, sessionResponse.FirstName, sessionResponse.Surname }
                .Where(part => !string.IsNullOrEmpty(part)));
        }

        // Emis does not have a logoff endpoint, returning successfully deleted
        public Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            return Task.FromResult((SessionLogoffResult) new SessionLogoffResult.Success(gpUserSession));
        }

        public Task<GpSessionRecreateResult> Recreate(string connectionToken, string odsCode, string nhsNumber, string patientId)
        {
            throw new NotImplementedException();
        }

        private bool IsSessionsPostResponseValid(SessionsPostResponse sessionsPostResponse)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(sessionsPostResponse.SessionId, nameof(sessionsPostResponse.SessionId))
                .IsNotNull(sessionsPostResponse.UserPatientLinks, nameof(sessionsPostResponse.UserPatientLinks))
                .IsValid();
        }
    }
}
