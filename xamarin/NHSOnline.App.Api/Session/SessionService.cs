using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Client;
using NHSOnline.App.Api.Client.Session;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Session
{
    internal sealed class SessionService : ISessionService, IApiCreateSessionResultVisitor<CreateSessionResult>
    {
        private readonly ILogger<SessionService> _logger;
        private readonly IApiClientEndpoint<ApiCreateSessionRequest, ApiCreateSessionResult> _createSessionEndpoint;

        public SessionService(
            ILogger<SessionService> logger,
            IApiClientEndpoint<ApiCreateSessionRequest, ApiCreateSessionResult> createSessionEndpoint)
        {
            _logger = logger;
            _createSessionEndpoint = createSessionEndpoint;
        }

        public async Task<CreateSessionResult> CreateSession(string authCode, string codeVerifier, Uri redirectUrl)
        {
            try
            {
                var request = new ApiCreateSessionRequest(authCode, codeVerifier, redirectUrl);
                var result = await _createSessionEndpoint.Call(request).ResumeOnThreadPool();

                return result.Accept(this);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CreateSession Failed");
                return new CreateSessionResult.Failed();
            }
        }

        public CreateSessionResult Visit(ApiCreateSessionResult.Success success)
        {
            var userSession = new UserSession(success.UserSessionResponse);
            return new CreateSessionResult.Created(userSession, success.Cookies);
        }

        public CreateSessionResult Visit(ApiCreateSessionResult.Failure failure)
            => new CreateSessionResult.Failed();

        public CreateSessionResult Visit(ApiCreateSessionResult.BadRequest badRequest)
            => new CreateSessionResult.BadRequest(badRequest.PfsErrorResponse.ServiceDeskReference);

        public CreateSessionResult Visit(ApiCreateSessionResult.Forbidden forbidden)
            => new CreateSessionResult.Forbidden(forbidden.PfsErrorResponse.ServiceDeskReference);

        public CreateSessionResult Visit(ApiCreateSessionResult.OdsCodeNotSupportedOrNoNhsNumber odsCodeNotSupportedOrNoNhsNumber)
            => new CreateSessionResult.OdsCodeNotSupportedOrNoNhsNumber(odsCodeNotSupportedOrNoNhsNumber.PfsErrorResponse.ServiceDeskReference);

        public CreateSessionResult Visit(ApiCreateSessionResult.FailedAgeRequirement failedAgeRequirement)
            => new CreateSessionResult.FailedAgeRequirement(failedAgeRequirement.PfsErrorResponse.ServiceDeskReference);

        public CreateSessionResult Visit(ApiCreateSessionResult.BadResponseFromUpstreamSystem badResponseFromUpstreamSystem)
            => new CreateSessionResult.BadResponseFromUpstreamSystem(badResponseFromUpstreamSystem.PfsErrorResponse.ServiceDeskReference);
    }
}