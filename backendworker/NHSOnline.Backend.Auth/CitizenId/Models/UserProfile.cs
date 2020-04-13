namespace NHSOnline.Backend.Auth.CitizenId.Models
{
    public class UserProfile
    {
        private readonly UserInfo _userInfo;

        public UserProfile(UserInfo userInfo, string accessToken)
        {
            _userInfo = userInfo;
            AccessToken = accessToken;
        }

        public string Im1ConnectionToken => _userInfo.Im1ConnectionToken;
        public string OdsCode => _userInfo.GpIntegrationCredentials.OdsCode;
        public string DateOfBirth => _userInfo.Birthdate;
        public string NhsNumber => _userInfo.NhsNumber;
        public string FamilyName => _userInfo.FamilyName;
        public string IdentityProofingLevel => _userInfo.IdentityProofingLevel;

        public string AccessToken { get; }
    }
}
