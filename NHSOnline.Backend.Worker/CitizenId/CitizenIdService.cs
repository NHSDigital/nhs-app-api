using System.Threading.Tasks;
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

        public CitizenIdService(ICitizenIdClient citizenIdClient)
        {
            _citizenIdClient = citizenIdClient;
        }

        public async Task<Option<UserProfile>> GetUserProfile(string authCode, string codeVerifier)
        {
            // Sanity-check input parameters - no point invoking CID endpoint if they are clearly invalid
            if (string.IsNullOrWhiteSpace(authCode) || string.IsNullOrWhiteSpace(codeVerifier))
            {
                return Option.None<UserProfile>();
            }

            // Exchange authorization code for bearer access token.
            var tokenResponse = await _citizenIdClient.ExchangeAuthToken(authCode, codeVerifier);
            if (!tokenResponse.HasSuccessStatusCode)
            {
                return Option.None<UserProfile>();
            }

            // Use the bearer access token to retrieve user profile.
            var userInfo = await _citizenIdClient.GetUserInfo(tokenResponse.Body.AccessToken);
            if (!userInfo.HasSuccessStatusCode)
            {
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
