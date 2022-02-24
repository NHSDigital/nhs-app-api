namespace NHSOnline.App.Api.Client.Session
{
    internal sealed class ApiCreateSessionRequestModel
    {
        public ApiCreateSessionRequestModel(string authCode, string codeVerifier, string referrer, string redirectUrl, string integrationReferrer)
        {
            AuthCode = authCode;
            CodeVerifier = codeVerifier;
            Referrer = referrer;
            RedirectUrl = redirectUrl;
            IntegrationReferrer = integrationReferrer;
        }

        public string AuthCode { get; }

        public string CodeVerifier { get; }

        public string RedirectUrl { get; }

        public string Referrer { get; }

        public string IntegrationReferrer { get; }
    }
}