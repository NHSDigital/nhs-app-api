using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session
{
    public class EmisSessionService : ISessionService, IEmisSessionService
    {
        private readonly IEmisClient _emisClient;
        private readonly ILogger<EmisSessionService> _logger;
        private readonly IEmisEnumMapper _emisEnumMapper;

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
            if (!endUserSessionResponse.HasSuccessStatusCode)
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(endUserSessionResponse);
                throw new EmisSessionResponseErrorException(new SessionCreateResult.SupplierSystemUnavailable());
            }

            return endUserSessionResponse.Body;
        }

        public async Task<SessionsPostResponse> SendSessionsRequest(string endUserSessionId, string connectionToken, string odsCode)
        {
            var sessionPostRequestModel = new SessionsPostRequest
            {
                AccessIdentityGuid = connectionToken,
                NationalPracticeCode = odsCode
            };

            var sessionsResponse = await _emisClient.SessionsPost(endUserSessionId, sessionPostRequestModel);
            if (!sessionsResponse.HasSuccessStatusCode)
            {
                if (InvalidTokenStatusCodes.Contains(sessionsResponse.StatusCode))
                {
                    _logger.LogEmisResponseIsForbidden();
                    _logger.LogEmisErrorResponse(sessionsResponse);
                    throw new EmisSessionResponseErrorException(new SessionCreateResult.InvalidIm1ConnectionToken());
                }

                _logger.LogEmisUnknownError(sessionsResponse);
                _logger.LogEmisErrorResponse(sessionsResponse);
                throw new EmisSessionResponseErrorException(new SessionCreateResult.SupplierSystemUnavailable());
            }

            return sessionsResponse.Body;
        }

        public async Task<SessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            try
            {
                _logger.LogEnter(nameof(Create));

                var endUserSessionResponse = await SendSessionsEndUserSessionPost();

                var sessionResponse =
                    await SendSessionsRequest(endUserSessionResponse.EndUserSessionId, connectionToken, odsCode);

                _logger.LogDebug("Emis session successfully created");

                var session = new EmisUserSession
                {
                    SessionId = sessionResponse.SessionId,
                    EndUserSessionId = endUserSessionResponse.EndUserSessionId,
                    UserPatientLinkToken = sessionResponse.ExtractUserPatientLinkToken(),
                    NhsNumber = nhsNumber,
                    OdsCode = odsCode,
                    AppointmentBookingReasonNecessity =  Necessity.Mandatory,
                    PrescriptionSpecialRequestNecessity = Necessity.Optional
                };

                try
                {
                    var headerParams = new EmisHeaderParameters(session);
                    var practiceResponse = await _emisClient.PracticeSettingsGet(headerParams, odsCode);

                    session.AppointmentBookingReasonNecessity =
                        _emisEnumMapper.MapNecessity(practiceResponse?.Body?.InputRequirements?.AppointmentBookingReason, Necessity.Mandatory);
                    
                    session.PrescriptionSpecialRequestNecessity =
                        _emisEnumMapper.MapNecessity(practiceResponse?.Body?.InputRequirements?.PrescribingComment, Necessity.Optional);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError(e, "Failed request to retrieve practice settings, HttpRequestException has been thrown.");
                }
                
                return new SessionCreateResult.SuccessfullyCreated(
                    FormatName(sessionResponse),
                    session
                );
            }
            catch (EmisSessionResponseErrorException responseError)
            {
                _logger.LogError(responseError,
                    "Failed request to create Emis user session,EmisSessionResponseErrorException has been thrown");
                return responseError.ErrorResult;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Failed request to create Emis user session,HttpRequestException has been thrown.");
                return new SessionCreateResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit(nameof(Create));
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
    }
}
