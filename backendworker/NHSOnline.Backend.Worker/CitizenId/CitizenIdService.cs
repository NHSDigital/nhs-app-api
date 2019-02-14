using System.Net;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

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
            IJwtTokenService<UserProfile> idTokenService, ILogger<CitizenIdService> logger)
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

                if(!isValid)
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

                result.UserProfile = _idTokenService
                    .ReadToken(tokenResponse.Body.IdToken,
                        signingKeys.ValueOrFailure())
                    .IfSome((userProfile) =>
                    {
                        result.StatusCode = HttpStatusCode.OK;
                        userProfile.AccessToken = tokenResponse.Body.AccessToken;
                        return Option.Some(userProfile);
                    })
                    .IfNone(() =>
                    {
                        result.StatusCode = HttpStatusCode.BadRequest;
                        return Option.None<UserProfile>();
                    });

                return result;
            }
            finally
            {
                _logger.LogExit();    
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
