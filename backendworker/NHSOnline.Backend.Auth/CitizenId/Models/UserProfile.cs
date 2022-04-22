namespace NHSOnline.Backend.Auth.CitizenId.Models
{
    public class UserProfile
    {
        private readonly UserInfo _userInfo;

        public UserProfile(UserInfo userInfo, string accessToken, string refreshToken, string idToken)
        {
            _userInfo = userInfo;
            AccessToken = accessToken;
            IdToken = idToken;
            RefreshToken = refreshToken;
        }

        public string Im1ConnectionToken => _userInfo.Im1ConnectionToken;
        public string OdsCode
        {
            get
            {
                if (!string.IsNullOrEmpty(_userInfo.GpRegistrationDetails?.OdsCode))
                {
                    return _userInfo.GpRegistrationDetails.OdsCode;
                }
                if (!string.IsNullOrEmpty(_userInfo.GpIntegrationCredentials?.OdsCode))
                {
                    return _userInfo.GpIntegrationCredentials.OdsCode;
                }
                return null;
            }
        }

        public string DateOfBirth => _userInfo.Birthdate;
        public string NhsNumber => _userInfo.NhsNumber;
        public string GivenName => _userInfo.GivenName;
        public string FamilyName => _userInfo.FamilyName;
        public string IdentityProofingLevel => _userInfo.IdentityProofingLevel;
        public string Email => _userInfo.Email;
        public string AccessToken { get; }
        public string IdToken { get; }
        public string RefreshToken { get; }
    }
}
