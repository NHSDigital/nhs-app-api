using System;

namespace NHSOnline.App.NhsLogin
{
    public sealed class CreateOnDemandGpSessionState
    {
        private readonly Uri _onGpDemandReturnUri;

        internal CreateOnDemandGpSessionState(
            Uri authoriseUri,
            Uri onGpDemandReturnUri)
        {
            _onGpDemandReturnUri = onGpDemandReturnUri;
            AuthoriseUri = authoriseUri;
        }

        public Uri AuthoriseUri { get; }

        public bool IsOnGpDemandReturn(Uri uri) => uri.LocalPath == _onGpDemandReturnUri.LocalPath;

        public OnGpDemandReturnCheckResult CheckOnGpDemandReturn(Uri uri)
        {
            if (!IsOnGpDemandReturn(uri))
            {
                throw new InvalidOperationException($"{uri} is not on-demand gp return uri - verify with {nameof(IsOnGpDemandReturn)} first");
            }

            return new OnGpDemandReturnCheckResult.Complete(uri);
        }
    }
}