using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public interface ISessionMapper
    {
        UserSession Map(HttpContext context, GpUserSession gpUserSession, CitizenIdUserSession citizenIdUserSession, string im1ConnectionToken);
    }

    public class SessionMapper : ISessionMapper
    {
        private readonly IAntiforgery _antiforgery;

        public SessionMapper(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }
        
        public UserSession Map(HttpContext context,  GpUserSession gpUserSession, CitizenIdUserSession citizenIdUserSession, string im1ConnectionToken)
        {
            return new UserSession()
            {
                CsrfToken = _antiforgery.GetTokens(context).RequestToken,
                GpUserSession = gpUserSession,
                CitizenIdUserSession = citizenIdUserSession,
                OrganDonationSessionId = Guid.NewGuid(),
                Im1ConnectionToken = im1ConnectionToken
            };
        }
    }
}