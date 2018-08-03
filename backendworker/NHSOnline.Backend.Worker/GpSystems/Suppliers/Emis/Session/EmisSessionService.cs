using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions;
using NHSOnline.Backend.Worker.Support.Logging;

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
                _logger.LogEnter(nameof(Create));

                var endUserSessionResponse = await SendSessionsEndUserSessionPost();

                var sessionResponse =
                    await SendSessionsRequest(endUserSessionResponse.EndUserSessionId, connectionToken, odsCode);

                var demographicsResponse = await SendDemographicsGetRequest(
                    sessionResponse.ExtractUserPatientLinkToken(),
                    sessionResponse.SessionId,
                    endUserSessionResponse.EndUserSessionId);

                _logger.LogDebug("Emis session successfully created");
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
    }
}
