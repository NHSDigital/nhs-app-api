using System;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.PfsApi.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    internal class CreateSessionRequest : ICreateSessionRequest
    {
        private readonly UserSessionRequest _model;

        internal CreateSessionRequest(UserSessionRequest model, string csrfToken, HttpContext httpContext)
        {
            _model = model;
            CsrfToken = csrfToken;
            HttpContext = httpContext;
        }

        public string AuthCode => _model.AuthCode;
        public string CodeVerifier => _model.CodeVerifier;
        public Uri RedirectUrl => new Uri(_model.RedirectUrl);
        public string CsrfToken { get; }
        public string IntegrationReferrer => _model.IntegrationReferrer;
        public string ReferrerOrigin => _model.ReferrerOrigin;
        public HttpContext HttpContext { get; }
        public string Referrer => _model.Referrer;
    }
}
