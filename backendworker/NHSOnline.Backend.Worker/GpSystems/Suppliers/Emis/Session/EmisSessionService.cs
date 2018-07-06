using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session
{
    public class EmisSessionService : ISessionService
    {
        private readonly IEmisClient _emisClient;
        private readonly ConfigurationSettings _settings;

        private static readonly HttpStatusCode[] InvalidTokenStatusCodes =
            { HttpStatusCode.Forbidden, HttpStatusCode.BadRequest };

        public EmisSessionService(IEmisClient emisClient, IOptions<ConfigurationSettings> settings)
        {
            _emisClient = emisClient;
            _settings = settings.Value;
        }

        public async Task<SessionCreateResult> Create(string connectionToken, string odsCode)
        {
            try
            {
                var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
                if (!endUserSessionResponse.HasSuccessStatusCode)
                {
                    return new SessionCreateResult.SupplierSystemUnavailable();
                }

                var endUserSessionId = endUserSessionResponse.Body.EndUserSessionId;
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
                        return new SessionCreateResult.InvalidIm1ConnectionToken();
                    }

                    return new SessionCreateResult.SupplierSystemUnavailable();
                }

                var sessionTimeoutInSeconds = _settings.DefaultSessionExpiryMinutes * 60;

                var sessionResponseBody = sessionsResponse.Body;

                return new SessionCreateResult.SuccessfullyCreated(
                    $"{sessionResponseBody.FirstName} {sessionResponseBody.Surname}",
                    new EmisUserSession
                    {
                        SessionId = sessionResponseBody.SessionId,
                        EndUserSessionId = endUserSessionId,
                        UserPatientLinkToken = sessionResponseBody.ExtractUserPatientLinkToken()
                    },
                    sessionTimeoutInSeconds
                );
            }
            catch (HttpRequestException)
            {
                return new SessionCreateResult.SupplierSystemUnavailable();
            }
        }
    }
}
