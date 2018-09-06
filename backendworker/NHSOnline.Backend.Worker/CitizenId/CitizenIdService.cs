using System;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.CitizenId.Models;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface ICitizenIdService
    {
        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        Task<GetUserProfileResult> GetUserProfile(string authCode, string codeVerifier, string redirectUrl);
    }

    public class CitizenIdService : ICitizenIdService
    {
        private readonly ICitizenIdClient _citizenIdClient;
        private readonly ICitizenIdSigningKeysService _citizenIdKeysService;
        private readonly IJwtTokenService<UserProfile> _idTokenService;
        private readonly ILogger<CitizenIdService>  _logger;

        public CitizenIdService(ICitizenIdClient citizenIdClient, ICitizenIdSigningKeysService citizenIdKeysService,
            IJwtTokenService<UserProfile> idTokenService, ILoggerFactory loggerFactory)
        {
            _citizenIdClient = citizenIdClient;
            _citizenIdKeysService = citizenIdKeysService;
            _idTokenService = idTokenService;
            _logger = loggerFactory.CreateLogger<CitizenIdService>();
        }

        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        public async Task<GetUserProfileResult> GetUserProfile(string authCode, string codeVerifier, string redirectUrl)
        {
            try
            {
                _logger.LogEnter(nameof(GetUserProfile));
                var result = new GetUserProfileResult();

                // Sanity-check input parameters - no point invoking CID endpoint if they are clearly invalid
                if (string.IsNullOrWhiteSpace(authCode) || string.IsNullOrWhiteSpace(codeVerifier) ||
                    string.IsNullOrWhiteSpace(redirectUrl))
                {
                    var missing = new List<string>();
                    if (string.IsNullOrEmpty(authCode))
                    {
                        missing.Add("authCode");
                    }

                    if (string.IsNullOrEmpty(codeVerifier))
                    {
                        missing.Add("codeVerifier");
                    }

                    if (string.IsNullOrEmpty(redirectUrl))
                    {
                        missing.Add("redirectUrl");
                    }

                    _logger.LogWarning($"Missing input parameters: {string.Join(", ", missing)}");
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.UserProfile = Option.None<UserProfile>();
                    return result;
                }

                var tokenTask = _citizenIdClient.ExchangeAuthToken(authCode, codeVerifier, redirectUrl);
                var keysTask = _citizenIdKeysService.GetSigningKeys();

                await Task.WhenAll(tokenTask, keysTask);

                var tokenResponse = tokenTask.Result;
                var signingKeys = keysTask.Result;
                if (!tokenResponse.HasSuccessStatusCode)
                {
                    LogError(tokenResponse, "Failed to exchange auth token for access token.");
                    result.StatusCode = MapCitizenIdErrorStatusCode(tokenResponse.StatusCode);
                    result.UserProfile = Option.None<UserProfile>();
                    return result;
                }

                if (!signingKeys.HasValue)
                {
                    _logger.LogError("Failed to get signing keys");
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.UserProfile = Option.None<UserProfile>();
                    return result;
                }

                result.UserProfile = _idTokenService.ReadToken(tokenResponse.Body.IdToken, signingKeys.ValueOrFailure());
                result.StatusCode = result.UserProfile.HasValue
                    ? HttpStatusCode.OK
                    : HttpStatusCode.BadRequest;
                return result;
            }
            finally
            {
                _logger.LogExit(nameof(GetUserProfile));    
            }
        }

        private void LogError<T>(CitizenIdClient.CitizenIdApiObjectResponse<T> apiResponse, string errorMessage)
        {
            _logger.LogError($"{errorMessage} Error code: '{apiResponse.ErrorResponse?.Error}'");
        }
        
        private static HttpStatusCode MapCitizenIdErrorStatusCode(HttpStatusCode code)
        {           
            return code == HttpStatusCode.BadRequest
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.BadGateway;
        }
    }
}
