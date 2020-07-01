namespace NHSOnline.App.Api.Client.Session
{
    internal sealed class UserSessionResponseModel
    {
        public string? Name { get; set; }

        public int SessionTimeout { get; set; }

        public string? OdsCode { get; set; }

        public string? Token { get; set; }

        public string? NhsNumber { get; set; }

        // Not in contract
        public string? DateOfBirth { get; set; }

        public string? AccessToken { get; set; }

        public bool Im1MessagingEnabled { get; set; }

        public string? ProofLevel { get; set; }
    }
}