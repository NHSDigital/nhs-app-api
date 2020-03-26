using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.GpSystems.SessionManager.Model;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public interface ISessionMapper
    {
        UserSession Map(HttpContext context, GpUserSession gpUserSession, GpSessionManagerCitizenIdUserSession citizenIdUserSession, string im1ConnectionToken);
    }

    public class SessionMapper : ISessionMapper
    {
        private readonly IAntiforgery _antiforgery;

        public SessionMapper(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public UserSession Map(HttpContext context,  GpUserSession gpUserSession, GpSessionManagerCitizenIdUserSession citizenIdUserSession, string im1ConnectionToken)
        {
            return new UserSession()
            {
                CsrfToken = _antiforgery.GetTokens(context).RequestToken,
                GpUserSession = gpUserSession,
                CitizenIdUserSession = new CitizenIdUserSession
                {
                    AccessToken = citizenIdUserSession.AccessToken,
                    DateOfBirth = citizenIdUserSession.DateOfBirth,
                    FamilyName = citizenIdUserSession.FamilyName,
                    IdTokenJti = citizenIdUserSession.IdTokenJti
                },
                OrganDonationSessionId = Guid.NewGuid(),
                Im1ConnectionToken = im1ConnectionToken
            };
        }
    }
}