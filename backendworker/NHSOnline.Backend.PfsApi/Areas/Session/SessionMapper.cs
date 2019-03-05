using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public interface ISessionMapper
    {
        UserSession Map(HttpContext context, GpUserSession gpUserSession, CitizenIdUserSession citizenIdUserSession);
    }

    public class SessionMapper : ISessionMapper
    {
        private readonly IAntiforgery _antiforgery;

        public SessionMapper(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }
        
        public UserSession Map(HttpContext context,  GpUserSession gpUserSession, CitizenIdUserSession citizenIdUserSession)
        {
            return new UserSession()
            {
                CsrfToken = _antiforgery.GetTokens(context).RequestToken,
                GpUserSession = gpUserSession,
                CitizenIdUserSession = citizenIdUserSession,
                OrganDonationSessionId = Guid.NewGuid()
            };
        }
    }
}