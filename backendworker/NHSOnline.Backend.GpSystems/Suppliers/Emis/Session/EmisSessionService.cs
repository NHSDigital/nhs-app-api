using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support;
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

        private const string StandardErrorMessage = "Failed request to create Emis user session";

        private static readonly HttpStatusCode[] InvalidTokenStatusCodes =
            { HttpStatusCode.Forbidden, HttpStatusCode.BadRequest };

        public EmisSessionService(IEmisClient emisClient, ILogger<EmisSessionService> logger, IEmisEnumMapper emisEnumMapper)
        {
            _emisClient = emisClient;
            _logger = logger;
            _emisEnumMapper = emisEnumMapper;
        }

        public async Task<SessionsEndUserSessionPostResponse> SendSessionsEndUserSessionPost()
        {
            var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
            if (!endUserSessionResponse.HasSuccessResponse)
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(endUserSessionResponse);
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.BadGateway());
            }

            var responseBody = endUserSessionResponse.Body;

            if (string.IsNullOrEmpty(responseBody.EndUserSessionId))
            {
                _logger.LogError("Gp system did not provide end user session Id");
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.BadGateway());
            }

            return responseBody;
        }

        public async Task<EmisClient.EmisApiObjectResponse<SessionsPostResponse>> SendSessionsRequest(string endUserSessionId, string accessIdentityGuid, string odsCode)
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
                    throw new EmisSessionResponseErrorException(new GpSessionCreateResult.Forbidden());
                }

                _logger.LogEmisUnknownError(sessionsResponse);
                _logger.LogEmisErrorResponse(sessionsResponse);
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.BadGateway());
            }

            var responseBody = sessionsResponse.Body;
            if (!IsSessionsPostResponseValid(responseBody))
            {
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.BadGateway());
            }

            return sessionsResponse;
        }

        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            try
            {
                _logger.LogEnter();

                var endUserSessionResponse = await SendSessionsEndUserSessionPost();

                string patientName;

                var session = new EmisUserSession
                {
                    Id = Guid.NewGuid(),
                    EndUserSessionId = endUserSessionResponse.EndUserSessionId,
                    NhsNumber = nhsNumber,
                    OdsCode = odsCode,
                    AppointmentBookingReasonNecessity =  Necessity.Mandatory,
                    PrescriptionSpecialRequestNecessity = Necessity.Optional
                };

                var emisRequestParameters = new EmisRequestParameters(session);

                var either = EmisConnectionTokenParser.Parse(connectionToken);
                var accessIdentityGuid = either.Match(guid => guid, ct => ct.AccessIdentityGuid);

                var sessionRequestTask = SendSessionsRequest(endUserSessionResponse.EndUserSessionId, accessIdentityGuid, odsCode);
                var practiceSettingsTask =  _emisClient.PracticeSettingsGet(emisRequestParameters, odsCode);
                await Task.WhenAll(sessionRequestTask, practiceSettingsTask);

                try
                {
                    var sessionResponse = new EmisRequestTaskChecker<EmisClient.EmisApiObjectResponse<SessionsPostResponse>>(_logger,
                        "SendSessionsRequest").Check(sessionRequestTask);

                    session.SessionId = sessionResponse.Body.SessionId;
                    session.UserPatientLinkToken = sessionResponse.Body.ExtractUserPatientLinkToken();
                    session.HasLinkedAccounts = sessionResponse.Body.HasLinkedPatients();
                    session.ProxyPatients = sessionResponse.Body.ExtractLinkedPatients()
                        .Select(x => new EmisProxyUserSession
                        {
                            Id = Guid.NewGuid(),
                            OdsCode = x.NationalPracticeCode,
                            UserPatientLinkToken = x.UserPatientLinkToken,
                        })
                        .ToList();

                    patientName = FormatName(sessionResponse.Body);

                    LogProxyInformation(sessionResponse.Body);

                    if (string.IsNullOrWhiteSpace(patientName))
                    {
                        _logger.LogError("No patient name found");
                        return new GpSessionCreateResult.BadGateway();
                    }

                    if (string.IsNullOrWhiteSpace(session.UserPatientLinkToken))
                    {
                        _logger.LogError("No EMIS userPatientLinkToken found");
                        _logger.LogEmisErrorResponse(sessionResponse);
                        return new GpSessionCreateResult.Forbidden();
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
                    _logger.LogError(e, $"{StandardErrorMessage}, HttpRequestException has been thrown.");
                    return new GpSessionCreateResult.BadGateway();
                }

                try
                {
                    var practiceResponse = new EmisRequestTaskChecker<EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>>(_logger, "GetPracticeDetails").Check(practiceSettingsTask);

                    session.AppointmentBookingReasonNecessity =
                        _emisEnumMapper.MapNecessity(practiceResponse?.Body?.InputRequirements?.AppointmentBookingReason, Necessity.Mandatory);

                    session.PrescriptionSpecialRequestNecessity =
                        _emisEnumMapper.MapNecessity(practiceResponse?.Body?.InputRequirements?.PrescribingComment, Necessity.Optional);

                    session.Im1MessagingEnabled =
                        practiceResponse?.Body?.Services?.PracticePatientCommunicationSupported ?? false;

                    session.Name = patientName;
                    
                    _logger.LogInformation($"Enabled services for practice {session.OdsCode}: {practiceResponse?.Body?.Services}");
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError(e, "Failed request to retrieve practice settings, HttpRequestException has been thrown.");
                }

                return new GpSessionCreateResult.Success(session);
            }
            catch (EmisSessionResponseErrorException responseError)
            {
                _logger.LogError(responseError,
                    $"{StandardErrorMessage}, {nameof(EmisSessionResponseErrorException)} has been thrown");
                return responseError.ErrorResult;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"{StandardErrorMessage}, HttpRequestException has been thrown.");
                return new GpSessionCreateResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private void LogProxyInformation(SessionsPostResponse sessionsPostResponse)
        {
            var userPatientLinks = sessionsPostResponse.ExtractLinkedPatients().ToList();
            var selfPatient = sessionsPostResponse.ExtractSelfPatient();
            var userOdsCode = selfPatient != null ? selfPatient.NationalPracticeCode : "";
            var differentOdsCode = userPatientLinks.Count(x => !x.NationalPracticeCode.Equals(userOdsCode, StringComparison.Ordinal));

            _logger.LogInformation(
                $"User has linked_accounts={userPatientLinks.Count}, with different_ods_codes_to_user={differentOdsCode}");
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

        private bool IsSessionsPostResponseValid(SessionsPostResponse sessionsPostResponse)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(sessionsPostResponse.SessionId, nameof(sessionsPostResponse.SessionId))
                .IsNotNull(sessionsPostResponse.UserPatientLinks, nameof(sessionsPostResponse.UserPatientLinks))
                .IsValid();
        }
    }
}
