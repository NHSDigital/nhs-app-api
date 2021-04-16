using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    internal sealed class CreateGpSessionOnDemandRequest : CreateSessionRequest, ICreateGpSessionOnDemandRequest
    {
        internal CreateGpSessionOnDemandRequest(
            P9UserSession userSession,
            UserSessionRequest model,
            string csrfToken,
            HttpContext httpContext) : base(model, csrfToken, httpContext)
        {
            UserSession = userSession;
        }

        public P9UserSession UserSession { get; }
    }
}
