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

        public bool IsAuthReturn(Uri uri) => uri.Scheme == _authReturnUri.Scheme;

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

            if (queryString["error"] != null)
            {
                var error = queryString["error"];
                _logger.LogError("NHS Login redirect error: {Error}; Uri: {Uri}", error, uri);
            }
            else
            {
                _logger.LogError("NHS Login redirect without code or error; Uri: {Uri}", uri);
            }

            return new AuthReturnCheckResult.Failed();
        }
    }
}