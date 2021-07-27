using System;
using System.Linq;
using System.Web;

namespace NHSOnline.App.NhsLogin
{
    public sealed class CreateOnDemandGpSessionState
    {
        private readonly Uri _onDemandGpReturnUri;

        internal CreateOnDemandGpSessionState(
            Uri authoriseUri,
            Uri onDemandGpReturnUri)
        {
            _onDemandGpReturnUri = onDemandGpReturnUri;
            AuthoriseUri = authoriseUri;
        }

        public Uri AuthoriseUri { get; }

        public bool IsOnDemandGpReturn(Uri uri) => uri.LocalPath == _onDemandGpReturnUri.LocalPath;

        public OnDemandGpReturnCheckResult CheckOnDemandGpReturn(Uri uri)
        {
            if (!IsOnDemandGpReturn(uri))
            {
                throw new InvalidOperationException($"{uri} is not on-demand gp return uri - verify with {nameof(IsOnDemandGpReturn)} first");
            }

            var queryParameters = HttpUtility.ParseQueryString(uri.Query);
            var queryDictionary = queryParameters.AllKeys.ToDictionary(key => key, key => queryParameters[key]);

            return new OnDemandGpReturnCheckResult.Complete(queryDictionary);
        }
    }
}