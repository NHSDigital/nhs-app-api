using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface ICitizenIdService
    {
        Task<Option<UserProfile>> GetUserProfile(string authCode, string codeVerifier);
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

        public async Task<Option<UserProfile>> GetUserProfile(string authCode, string codeVerifier)
        {
            _logger.LogDebug("Starting GetUserProfile");
            // Sanity-check input parameters - no point invoking CID endpoint if they are clearly invalid
            if (string.IsNullOrWhiteSpace(authCode) || string.IsNullOrWhiteSpace(codeVerifier))
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
                
                _logger.LogWarning($"Missing input parameters: {missing.Join(", ")}");
                return Option.None<UserProfile>();
            }

            // Exchange authorization code for bearer access token.
            var tokenResponse = await _citizenIdClient.ExchangeAuthToken(authCode, codeVerifier);
            if (!tokenResponse.HasSuccessStatusCode)
            {
                _logger.LogError($"Failed to exchange auth token. {tokenResponse.ErrorResponse.SerializeJson()}");
                return Option.None<UserProfile>();
            }

            // Use the bearer access token to retrieve user profile.
            var userInfo = await _citizenIdClient.GetUserInfo(tokenResponse.Body.AccessToken);
            if (!userInfo.HasSuccessStatusCode)
            {
                _logger.LogError($"Failed to get user information from Citizen Id. {userInfo.ErrorResponse.SerializeJson()}");
                return Option.None<UserProfile>();
            }

            var userProfile = new UserProfile
            {
                Im1ConnectionToken = userInfo.Body.Im1ConnectionToken,
                OdsCode = userInfo.Body.OdsCode
            };

            return Option.Some(userProfile);
        }
    }
}
