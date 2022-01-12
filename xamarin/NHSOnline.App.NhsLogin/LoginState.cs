using System;
using System.Web;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.NhsLogin
{
    public sealed class LoginState
    {
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
            if (errorDescription != null && "ConsentNotGiven".Equals(errorDescription, StringComparison.Ordinal))
            {
                return new AuthReturnCheckResult.TermsAndConditionsDeclined();
            }

            string errorLogMessage;
            if (queryString["error"] != null)
            {
                var error = queryString["error"];
                errorLogMessage = $"NHS login redirect error: {error}; Uri: {uri}";
            }
            else
            {
                errorLogMessage = $"NHS login redirect without code or error; Uri: {uri}";
            }

            return new AuthReturnCheckResult.Failed(errorLogMessage);
        }
    }
}