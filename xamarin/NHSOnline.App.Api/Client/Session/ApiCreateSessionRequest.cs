using System;

namespace NHSOnline.App.Api.Client.Session
{
    public sealed class ApiCreateSessionRequest
    {
        private readonly ApiCreateSessionRequestModel _model;

        public ApiCreateSessionRequest(string authCode, string codeVerifier, Uri redirectUrl)
        {
            _model = new ApiCreateSessionRequestModel(authCode, codeVerifier, redirectUrl.ToString());
        }

        internal ApiCreateSessionRequestModel CreateModel() => _model;
    }
}
