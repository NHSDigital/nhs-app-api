using System;
using System.Diagnostics.CodeAnalysis;
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

        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        public async Task<GetUserProfileResult> GetUserProfile(string authCode, string codeVerifier, string redirectUrl)
        {
            _logger.LogEnter();
            var result = new GetUserProfileResult();

            try
            {
                var isValid = new ValidateAndLog(_logger).IsNotNullOrWhitespace(authCode, nameof(authCode))
                    .IsNotNullOrWhitespace(codeVerifier, nameof(codeVerifier))
                    .IsNotNullOrWhitespace(redirectUrl, nameof(redirectUrl))
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

                _idTokenService.ReadToken(tokenResponse.Body.IdToken, signingKeys.ValueOrFailure())
                    .IfSome(idToken =>
                    {
                        var resultTask = GetUserProfile(tokenResponse.Body.AccessToken, idToken.Subject);
                        resultTask.Wait();

                        result = resultTask.Result;
                        result.IdTokenJti = idToken.Jti;

                        return Option.Some(idToken);
                    })
                    .IfNone(() =>
                    {
                        _logger.LogError("Failed to read ID Token");
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.UserProfile = Option.None<UserProfile>();
                        return Option.None<IdToken>();
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

                return await GetUserProfile(accessToken, accessTokenObject.Subject);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<GetUserProfileResult> GetUserProfile(string accessToken, string subject)
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

            var userProfile = new UserProfile
            {
                NhsNumber = userInfo.Body.NhsNumber,
                OdsCode = userInfo.Body.GpIntegrationCredentials.OdsCode,
                Im1ConnectionToken = userInfo.Body.Im1ConnectionToken,
                FamilyName = userInfo.Body.FamilyName,
                DateOfBirth = userInfo.Body.Birthdate,
                AccessToken = accessToken
            };

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