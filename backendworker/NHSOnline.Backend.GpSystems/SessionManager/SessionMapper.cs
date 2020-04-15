using System;
using Microsoft.Extensions.Primitives;
using NHSOnline.Backend.GpSystems.SessionManager.Model;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public class SessionMapper : ISessionMapper
    {
        public P9UserSession Map(StringValues csrfToken, GpUserSession gpUserSession, GpSessionManagerCitizenIdUserSession citizenIdUserSession, string im1ConnectionToken)
        {
            return new P9UserSession
            {
                CsrfToken = csrfToken,
                GpUserSession = gpUserSession,
                CitizenIdUserSession = new CitizenIdUserSession
                {
                    AccessToken = citizenIdUserSession.AccessToken,
                    DateOfBirth = citizenIdUserSession.DateOfBirth,
                    FamilyName = citizenIdUserSession.FamilyName,
                    IdTokenJti = citizenIdUserSession.IdTokenJti,
                    ProofLevel = citizenIdUserSession.ProofLevel
                },
                OrganDonationSessionId = Guid.NewGuid(),
                Im1ConnectionToken = im1ConnectionToken
            };
        }
    }
}