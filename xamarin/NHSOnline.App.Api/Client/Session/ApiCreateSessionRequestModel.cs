namespace NHSOnline.App.Api.Client.Session
{
    internal sealed class ApiCreateSessionRequestModel
    {
        public ApiCreateSessionRequestModel(string authCode, string codeVerifier, string referrer, string redirectUrl)
        {
            AuthCode = authCode;
            CodeVerifier = codeVerifier;
            Referrer = referrer;
            RedirectUrl = redirectUrl;
        }

        public string AuthCode { get; }

        public string CodeVerifier { get; }

        public string RedirectUrl { get; }

        public string Referrer { get; }
    }
}