using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
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

        private static readonly List<(Type? Exception, Regex Regex)> CreateSessionRetries = new()
        {
            (typeof(HttpRequestException), new Regex(".*The Internet connection appears to be offline.*")),
            (typeof(HttpRequestException), new Regex(".*A server with the specified hostname could not be found.*")),
            (typeof(HttpRequestException), new Regex(".*bad URL.*")),
            (typeof(HttpRequestException), new Regex(".*Could not connect to the server.*")),
            (typeof(HttpRequestException), new Regex(".*An SSL error has occurred and a secure connection to the server cannot be made.*")),
            (typeof(WebException), new Regex(".*Failed to connect to api.nhsapp.service.nhs.uk.*")),
            (null, new Regex(".*failed to connect to api.nhsapp.service.nhs.uk.*")),
            (null, new Regex(".*SSL handshake aborted.*")),
            (null, new Regex(".*Read error:.*: Failure in SSL library, usually a protocol error.*")),
        };

        public SessionService(
            ILogger<SessionService> logger,
            IApiClientEndpoint<ApiCreateSessionRequest, ApiCreateSessionResult> createSessionEndpoint)
        {
            _logger = logger;
            _createSessionEndpoint = createSessionEndpoint;
        }

        public Task<CreateSessionResult> CreateSession(string authCode, string codeVerifier, string referrer, Uri redirectUrl, string integrationReferrer)
            => CreateSession(authCode, codeVerifier, referrer, redirectUrl, integrationReferrer, true);

        public CreateSessionResult Visit(ApiCreateSessionResult.Success success)
        {
            var userSession = new UserSession(success.UserSessionResponse);
            return new CreateSessionResult.Created(userSession, success.Cookies);
        }

        public CreateSessionResult Visit(ApiCreateSessionResult.Failure failure)
            => new CreateSessionResult.Failed();

        public CreateSessionResult Visit(ApiCreateSessionResult.BadRequest badRequest)
            => new CreateSessionResult.BadRequest(badRequest.PfsErrorResponse.ServiceDeskReference);

        public CreateSessionResult Visit(ApiCreateSessionResult.OdsCodeNotSupported odsCodeNotSupported)
            => new CreateSessionResult.OdsCodeNotSupported(odsCodeNotSupported.PfsErrorResponse.ServiceDeskReference);

        public CreateSessionResult Visit(ApiCreateSessionResult.OdsCodeNotFound odsCodeNotFound)
            => new CreateSessionResult.OdsCodeNotFound(odsCodeNotFound.PfsErrorResponse.ServiceDeskReference);

        public CreateSessionResult Visit(ApiCreateSessionResult.NoNhsNumber noNhsNumber)
            => new CreateSessionResult.NoNhsNumber(noNhsNumber.PfsErrorResponse.ServiceDeskReference);

        public CreateSessionResult Visit(ApiCreateSessionResult.FailedAgeRequirement failedAgeRequirement)
            => new CreateSessionResult.FailedAgeRequirement(failedAgeRequirement.PfsErrorResponse.ServiceDeskReference);

        public CreateSessionResult Visit(ApiCreateSessionResult.BadResponseFromUpstreamSystem badResponseFromUpstreamSystem)
            => new CreateSessionResult.BadResponseFromUpstreamSystem(badResponseFromUpstreamSystem.PfsErrorResponse.ServiceDeskReference);

        public CreateSessionResult Visit(ApiCreateSessionResult.UpstreamSystemTimeout upstreamSystemTimeout)
            => new CreateSessionResult.UpstreamSystemTimeout(upstreamSystemTimeout.PfsErrorResponse.ServiceDeskReference);

        public CreateSessionResult Visit(ApiCreateSessionResult.InternalServerError internalServerError)
            => new CreateSessionResult.InternalServerError(internalServerError.PfsErrorResponse.ServiceDeskReference);

        private async Task<CreateSessionResult> CreateSession(
            string authCode, string codeVerifier, string referrer,
            Uri redirectUrl, string integrationReferrer, bool shouldRetry)
        {
            try
            {
                var request = new ApiCreateSessionRequest(authCode, codeVerifier, referrer, redirectUrl, integrationReferrer);
                var result = await _createSessionEndpoint.Call(request, CancellationToken.None).ResumeOnThreadPool();

                return result.Accept(this);
            }
            catch (Exception e)
            {
                if (shouldRetry && ShouldRetryOnException(e))
                {
                    _logger.LogError(e, "CreateSession Failed - Retrying");
                    return await CreateSession(authCode, codeVerifier, referrer, redirectUrl, integrationReferrer, false).ResumeOnThreadPool();
                }

                _logger.LogError(e, "CreateSession Failed");
                return new CreateSessionResult.Failed();
            }
        }

        private static bool ShouldRetryOnException(Exception exception) =>
            CreateSessionRetries.Any(retry =>
                (retry.Exception is null || exception.GetType() == retry.Exception) && retry.Regex.IsMatch(exception.Message));
    }
}