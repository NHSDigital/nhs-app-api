using System;

namespace NHSOnline.App.Api.Client.Session
{
    public sealed class ApiCreateSessionRequest
    {
        private readonly ApiCreateSessionRequestModel _model;

        public ApiCreateSessionRequest(string authCode, string codeVerifier, string referrer, Uri redirectUrl, string integrationReferrer)
        {
            _model = new ApiCreateSessionRequestModel(authCode, codeVerifier, referrer, redirectUrl.ToString(), integrationReferrer);
        }

        internal ApiCreateSessionRequestModel CreateModel() => _model;
    }
}