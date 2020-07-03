namespace NHSOnline.App.Api.Client.Session
{
    internal sealed class UserSessionResponse
    {
        public UserSessionResponse(
            string? name,
            int sessionTimeout,
            string? odsCode,
            string? token,
            string? nhsNumber,
            string? dateOfBirth,
            string? accessToken,
            bool im1MessagingEnabled,
            string? proofLevel)
        {
            Name = name;
            SessionTimeout = sessionTimeout;
            OdsCode = odsCode;
            Token = token;
            NhsNumber = nhsNumber;
            DateOfBirth = dateOfBirth;
            AccessToken = accessToken;
            Im1MessagingEnabled = im1MessagingEnabled;
            ProofLevel = proofLevel;
        }

        public string? Name { get; }

        public int SessionTimeout { get; }

        public string? OdsCode { get; }

        public string? Token { get; }

        public string? NhsNumber { get; }

        public string? DateOfBirth { get; }

        public string? AccessToken { get; }

        public bool Im1MessagingEnabled { get; }

        public string? ProofLevel { get; }
    }
}