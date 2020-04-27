namespace NHSOnline.Backend.Auth.CitizenId.Models
{
    public class UserProfile
    {
        private readonly UserInfo _userInfo;
        
        public UserProfile(UserInfo userInfo, string accessToken, string refreshToken)
        {
            _userInfo = userInfo;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public string Im1ConnectionToken => _userInfo.Im1ConnectionToken;
        public string OdsCode => _userInfo.GpIntegrationCredentials.OdsCode;
        public string DateOfBirth => _userInfo.Birthdate;
        public string NhsNumber => _userInfo.NhsNumber;
        public string GivenName => _userInfo.GivenName;
        public string FamilyName => _userInfo.FamilyName;
        public string IdentityProofingLevel => _userInfo.IdentityProofingLevel;

        public string AccessToken { get; }
        public string RefreshToken { get; }
    }
}
