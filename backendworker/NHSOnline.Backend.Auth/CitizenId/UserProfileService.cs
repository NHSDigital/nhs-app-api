using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly ICitizenIdService _citizenIdService;
        private UserProfile _userProfile;
        public UserProfileService(IAccessTokenProvider accessTokenProvider, ICitizenIdService citizenIdService)
        {
            _accessTokenProvider = accessTokenProvider;
            _citizenIdService = citizenIdService;
        }

        internal void SetUserProfile(UserProfile userProfile) => _userProfile = userProfile;

        public UserProfile GetExistingUserProfileOrThrow(string context)
        {
            if (_userProfile == null)
            {
                throw new InvalidOperationException(
                    $"{context}: Required user profile but no profile has been set");
            }

            return _userProfile;
        }
        public async Task<Option<UserProfile>> GetUserProfile()
        {
            var userProfileResult = await _citizenIdService.GetUserProfile(_accessTokenProvider.AccessToken.ToString());
            return userProfileResult.UserProfile;
        }
    }
}