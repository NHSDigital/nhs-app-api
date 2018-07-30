using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface ICitizenIdService
    {
        Task<Option<UserProfile>> GetUserProfile(string authCode, string codeVerifier, string redirectUrl);
    }

    public class CitizenIdService : ICitizenIdService
    {
        private readonly ICitizenIdClient _citizenIdClient;
        private readonly ILogger<CitizenIdService>  _logger;

        public CitizenIdService(ICitizenIdClient citizenIdClient, ILoggerFactory loggerFactory)
        {
            _citizenIdClient = citizenIdClient;
            _logger = loggerFactory.CreateLogger<CitizenIdService>();
        }

        public async Task<Option<UserProfile>> GetUserProfile(string authCode, string codeVerifier, string redirectUrl)
        {
            try
            {
                _logger.LogEnter(nameof(GetUserProfile));
                
                // Sanity-check input parameters - no point invoking CID endpoint if they are clearly invalid
                if (string.IsNullOrWhiteSpace(authCode) || string.IsNullOrWhiteSpace(codeVerifier) || string.IsNullOrWhiteSpace(redirectUrl))
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
                    return Option.None<UserProfile>();
                }
    
                // Exchange authorisation code for bearer access token.
                var tokenResponse = await _citizenIdClient.ExchangeAuthToken(authCode, codeVerifier, redirectUrl);
                if (!tokenResponse.HasSuccessStatusCode)
                {
                    LogError(tokenResponse, "Failed to exchange auth token for access token.");
                    return Option.None<UserProfile>();
                }
    
                // Use the bearer access token to retrieve user profile.
                var userInfo = await _citizenIdClient.GetUserInfo(tokenResponse.Body.AccessToken);
                if (!userInfo.HasSuccessStatusCode)
                {
                    LogError(userInfo, "Failed to get user information from Citizen Id.");
                    return Option.None<UserProfile>();
                }
    
                var userProfile = new UserProfile
                {
                    Im1ConnectionToken = userInfo.Body.Im1ConnectionToken,
                    OdsCode = userInfo.Body.OdsCode
                };
                
                return Option.Some(userProfile);
            }
            finally
            {
                _logger.LogExit(nameof(GetUserProfile));    
            }
        }

        private void LogError<T>(CitizenIdClient.CitizenIdApiObjectResponse<T> apiResponse, string errorMessage)
        {
            _logger.LogError($"{errorMessage} Error code: '{apiResponse.ErrorResponse?.Error}', Error message: '{apiResponse.ErrorResponse?.ErrorDescription}'");
        }
    }
}
