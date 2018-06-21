using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session
{
    public class EmisSessionService : ISessionService
    {
        private readonly IEmisClient _emisClient;
        private readonly IEmisDemographicsMapper _demographicsMapper;
        private readonly ILogger<EmisSessionService> _logger;

        private static readonly HttpStatusCode[] InvalidTokenStatusCodes =
            { HttpStatusCode.Forbidden, HttpStatusCode.BadRequest };

        public EmisSessionService(IEmisClient emisClient, IEmisDemographicsMapper demographicsMapper, ILogger<EmisSessionService> logger)
        {
            _emisClient = emisClient;
            _demographicsMapper = demographicsMapper;
            _logger = logger;
        }

        private class EmisSessionResponseErrorException : Exception
        {
            public SessionCreateResult ErrorResult{ get; }

            public EmisSessionResponseErrorException(SessionCreateResult erroResult)
            {
                ErrorResult = erroResult;
            }
        }
        private async Task<SessionsEndUserSessionPostResponse> SendSessionsEndUserSessionPost()
        {
            var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
            if (!endUserSessionResponse.HasSuccessStatusCode)
            {
                _logger.LogEmisResponseIsForbidden();
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
                    throw new EmisSessionResponseErrorException(new SessionCreateResult.InvalidIm1ConnectionToken());
                }

                _logger.LogEmisUnknownError(sessionsResponse);
                throw new EmisSessionResponseErrorException(new SessionCreateResult.SupplierSystemUnavailable());
            }

            return sessionsResponse.Body;
        }

        private async Task<DemographicsGetResponse> SendDemographicsGetRequest(string userPatientLinkToken, string sessionId, string endUserSessionId)
        {
            var demographicsResponse = await _emisClient.DemographicsGet(userPatientLinkToken, sessionId, endUserSessionId);
            if (!demographicsResponse.HasSuccessStatusCode)
            {
                if (InvalidTokenStatusCodes.Contains(demographicsResponse.StatusCode))
                {
                    _logger.LogEmisResponseIsForbidden();
                    throw new EmisSessionResponseErrorException(new SessionCreateResult.InvalidIm1ConnectionToken());
                }

                _logger.LogEmisUnknownError(demographicsResponse);
                throw new EmisSessionResponseErrorException(new SessionCreateResult.SupplierSystemUnavailable());
            }

            return demographicsResponse.Body;
        }

        public async Task<SessionCreateResult> Create(string connectionToken, string odsCode)
        {
            try
            {
                var endUserSessionResponse = await SendSessionsEndUserSessionPost();

                var sessionResponse = await SendSessionsRequest(endUserSessionResponse.EndUserSessionId, connectionToken, odsCode);

                var demographicsResponse = await SendDemographicsGetRequest(
                    sessionResponse.ExtractUserPatientLinkToken(),
                    sessionResponse.SessionId,
                    endUserSessionResponse.EndUserSessionId);

                return new SessionCreateResult.SuccessfullyCreated(
                    $"{sessionResponse.FirstName} {sessionResponse.Surname}",
                    new EmisUserSession
                    {
                        SessionId = sessionResponse.SessionId,
                        EndUserSessionId = endUserSessionResponse.EndUserSessionId,
                        UserPatientLinkToken = sessionResponse.ExtractUserPatientLinkToken(),
                        NhsNumber = _demographicsMapper.Map(demographicsResponse).NhsNumber
                    }
                );
            }
            catch (EmisSessionResponseErrorException responseError)
            {
                return responseError.ErrorResult;
            }
            catch (HttpRequestException)
            {
                return new SessionCreateResult.SupplierSystemUnavailable();
            }
        }
    }
}
