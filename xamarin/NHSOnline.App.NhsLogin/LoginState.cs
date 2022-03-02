using System;
using System.Web;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.NhsLogin
{
    public sealed class LoginState
    {
        private const string NhsLoginErrorSignatureInvalid = "signature_invalid";
        private const string NhsLoginErrorConsentNotGiven = "ConsentNotGiven";
        private readonly ILogger _logger;
        private readonly Uri _authReturnUri;

        internal LoginState(
            ILogger logger,
            Uri authoriseUri,
            Uri authReturnUri)
        {
            _logger = logger;
            _authReturnUri = authReturnUri;
            AuthoriseUri = authoriseUri;
        }

        public Uri AuthoriseUri { get; }

        public bool IsAuthReturn(Uri uri) => uri.IsSameAddress(_authReturnUri);

        public AuthReturnCheckResult CheckAuthReturn(Uri uri)
        {
            if (!IsAuthReturn(uri))
            {
                throw new InvalidOperationException($"{uri} is not auth return uri - verify with {nameof(IsAuthReturn)} first");
            }

            var queryString = HttpUtility.ParseQueryString(uri.Query);

            if (queryString["code"] != null)
            {
                var code = queryString["code"];
                return new AuthReturnCheckResult.Authorised(_authReturnUri, code);
            }

            var errorDescription = queryString["error_description"];
            if (errorDescription is NhsLoginErrorConsentNotGiven)
            {
                return new AuthReturnCheckResult.TermsAndConditionsDeclined();
            }

            var error = queryString["error"];
            string errorLogMessage = queryString["error"] == null ?
                $"NHS login redirect without code or error; Uri: {uri}"
                : $"NHS login redirect error: {error}; Uri: {uri}";

            if (error is NhsLoginErrorSignatureInvalid)
            {
                return new AuthReturnCheckResult.SignatureInvalid();
            }

            return new AuthReturnCheckResult.Failed(errorLogMessage);
        }
    }
}