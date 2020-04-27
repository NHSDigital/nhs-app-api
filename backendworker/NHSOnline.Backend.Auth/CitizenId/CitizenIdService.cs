using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class CitizenIdService : ICitizenIdService
    {
        private readonly ICitizenIdClient _citizenIdClient;
        private readonly ICitizenIdSigningKeysService _citizenIdKeysService;
        private readonly IJwtTokenService<IdToken> _idTokenService;
        private readonly ILogger<CitizenIdService> _logger;

        public CitizenIdService(ICitizenIdClient citizenIdClient, ICitizenIdSigningKeysService citizenIdKeysService,
            IJwtTokenService<IdToken> idTokenService, ILogger<CitizenIdService> logger)
        {
            _citizenIdClient = citizenIdClient;
            _citizenIdKeysService = citizenIdKeysService;
            _idTokenService = idTokenService;
            _logger = logger;
        }

        public async Task<GetUserProfileResult> GetUserProfile(string authCode, string codeVerifier, Uri redirectUrl)
        {
            _logger.LogEnter();
            var result = new GetUserProfileResult();

            try
            {
                var isValid = new ValidateAndLog(_logger).IsNotNullOrWhitespace(authCode, nameof(authCode))
                    .IsNotNullOrWhitespace(codeVerifier, nameof(codeVerifier))
                    .IsNotNull(redirectUrl, nameof(redirectUrl))
                    .IsValid();

                if (!isValid)
                {
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

                await _idTokenService
                    .ReadToken(tokenResponse.Body.IdToken, signingKeys.ValueOrFailure())
                    .IfSome(async idToken =>
                    {
                        result = await GetUserProfileFromCitizenId(tokenResponse.Body.AccessToken, idToken.Subject, tokenResponse.Body.RefreshToken);
                        result.IdTokenJti = idToken.Jti;
                    })
                    .IfNone(() =>
                    {
                        _logger.LogError("Failed to read ID Token");
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.UserProfile = Option.None<UserProfile>();
                        return Task.CompletedTask;
                    });

                return result;
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetUserProfileResult> GetUserProfile(string accessToken)
        {
            _logger.LogEnter();
            var result = new GetUserProfileResult();

            try
            {
                var isValid = new ValidateAndLog(_logger)
                    .IsNotNullOrWhitespace(accessToken, nameof(accessToken))
                    .IsValid();

                if (!isValid)
                {
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.UserProfile = Option.None<UserProfile>();
                    return result;
                }

                var accessTokenObject = AccessToken.Parse(_logger, accessToken);

                return await GetUserProfileFromCitizenId(accessToken, accessTokenObject.Subject);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<RefreshAccessTokenResult> RefreshAccessToken(string refreshToken)
        {
            _logger.LogEnter();

            try
            {
                var result = await _citizenIdClient.RefreshAccessToken(refreshToken);

                if (!result.HasSuccessStatusCode)
                {
                    _logger.LogError($"Failed to refresh access token, due to {result.StatusCode} response");
                    return new RefreshAccessTokenResult.BadGateway();
                }

                return new RefreshAccessTokenResult.Success(result.Body.AccessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error refreshing access token", ex);
                return new RefreshAccessTokenResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<GetUserProfileResult> GetUserProfileFromCitizenId(string accessToken, string subject, string refreshToken = null)
        {
            var userInfo = await _citizenIdClient.GetUserInfo(accessToken);

            if (!userInfo.HasSuccessStatusCode)
            {
                LogError(userInfo, "Failed to retrieve User Profile.");
                return new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    UserProfile = Option.None<UserProfile>()
                };
            }

            if (!subject.Equals(userInfo.Body.Subject, StringComparison.Ordinal))
            {
                _logger.LogError("Value of subject claim differed between Token and UserInfo responses");
                return new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    UserProfile = Option.None<UserProfile>()
                };
            }

            var userProfile = new UserProfile(userInfo.Body, accessToken, refreshToken);

            return new GetUserProfileResult
            {
                StatusCode = HttpStatusCode.OK,
                UserProfile = Option.Some(userProfile)
            };
        }

        private void LogError<T>(CitizenIdApiObjectResponse<T> apiResponse, string errorMessage)
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