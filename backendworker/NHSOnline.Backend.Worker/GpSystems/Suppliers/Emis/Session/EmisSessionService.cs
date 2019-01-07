using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session
{
    public class EmisSessionService : ISessionService, IEmisSessionService
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
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.SupplierSystemUnavailable());
            }

            var responseBody = endUserSessionResponse.Body;

            if (string.IsNullOrEmpty(responseBody.EndUserSessionId))
            {
                _logger.LogError("Gp system did not provide end user session Id");
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.SupplierSystemBadResponse());
            }

            return responseBody;
        }

        public async Task<SessionsPostResponse> SendSessionsRequest(string endUserSessionId, string accessIdentityGuid, string odsCode)
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
                    throw new EmisSessionResponseErrorException(new GpSessionCreateResult.InvalidIm1ConnectionToken());
                }

                _logger.LogEmisUnknownError(sessionsResponse);
                _logger.LogEmisErrorResponse(sessionsResponse);
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.SupplierSystemUnavailable());
            }

            var responseBody = sessionsResponse.Body;
            if (!IsSessionsPostResponseValid(responseBody))
            {
                throw new EmisSessionResponseErrorException(new GpSessionCreateResult.SupplierSystemBadResponse());
            }

            return sessionsResponse.Body;
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
                    EndUserSessionId = endUserSessionResponse.EndUserSessionId,
                    NhsNumber = nhsNumber,
                    OdsCode = odsCode,
                    AppointmentBookingReasonNecessity =  Necessity.Mandatory,
                    PrescriptionSpecialRequestNecessity = Necessity.Optional
                };

                var headerParams = new EmisHeaderParameters(session);

                var either = EmisConnectionTokenParser.Parse(connectionToken);
                var accessIdentityGuid = either.Match(guid => guid, ct => ct.AccessIdentityGuid);

                var sessionRequestTask = SendSessionsRequest(endUserSessionResponse.EndUserSessionId, accessIdentityGuid, odsCode);
                var practiceSettingsTask =  _emisClient.PracticeSettingsGet(headerParams, odsCode);
                await Task.WhenAll(sessionRequestTask, practiceSettingsTask);

                try
                {
                    var sessionResponse = new EmisRequestTaskChecker<SessionsPostResponse>(_logger, "SendSessionsRequest").Check(sessionRequestTask);
                    session.SessionId = sessionResponse.SessionId;
                    session.UserPatientLinkToken = sessionResponse.ExtractUserPatientLinkToken();  
                    patientName = FormatName(sessionResponse);

                    if (string.IsNullOrWhiteSpace(patientName))
                    {
                        _logger.LogError("No patient name found");
                        return new GpSessionCreateResult.SupplierSystemBadResponse();
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
                    return new GpSessionCreateResult.SupplierSystemUnavailable();
                }
                
                try
                {
                    var practiceResponse = new EmisRequestTaskChecker<EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>>(_logger, "GetPracticeDetails").Check(practiceSettingsTask);
             
                    session.AppointmentBookingReasonNecessity =
                        _emisEnumMapper.MapNecessity(practiceResponse?.Body?.InputRequirements?.AppointmentBookingReason, Necessity.Mandatory);
                    
                    session.PrescriptionSpecialRequestNecessity =
                        _emisEnumMapper.MapNecessity(practiceResponse?.Body?.InputRequirements?.PrescribingComment, Necessity.Optional);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError(e, "Failed request to retrieve practice settings, HttpRequestException has been thrown.");
                }
                
                return new GpSessionCreateResult.SuccessfullyCreated(patientName, session);
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
                return new GpSessionCreateResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private static string FormatName(SessionsPostResponse sessionResponse)
        {
            return string.Join(" ", new[] { sessionResponse.Title, sessionResponse.FirstName, sessionResponse.Surname }
                .Where(part => !string.IsNullOrEmpty(part)));
        }

        // Emis does not have a logoff endpoint, returning successfully deleted
        public Task<SessionLogoffResult> Logoff(UserSession userSession)
        {
            return Task.FromResult((SessionLogoffResult) new SessionLogoffResult.SuccessfullyDeleted(userSession));
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
