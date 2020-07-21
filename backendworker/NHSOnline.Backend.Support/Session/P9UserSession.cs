using System;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Support.Session
{
    public sealed class P9UserSession: P5UserSession
    {
        private P9UserSession()
        { }

        public P9UserSession(string csrfToken,
            CitizenIdUserSession citizenIdUserSession,
            string nhsNumber,
            string im1ConnectionToken,
            string userSessionCreateReferenceCode) :  base(csrfToken, citizenIdUserSession)
        {
            NhsNumber = nhsNumber;
            Im1ConnectionToken = im1ConnectionToken;
            UserSessionCreateReferenceCode = userSessionCreateReferenceCode;
            OrganDonationSessionId = Guid.NewGuid();
        }

        public P9UserSession(
            string csrfToken,
            string nhsNumber,
            CitizenIdUserSession citizenIdUserSession,
            GpUserSession gpUserSession,
            string im1ConnectionToken)
            : base(csrfToken, citizenIdUserSession)
        {
            GpUserSession = gpUserSession;
            NhsNumber = nhsNumber;
            Im1ConnectionToken = im1ConnectionToken;
            OrganDonationSessionId = Guid.NewGuid();
        }

        public GpUserSession GpUserSession { get; set; }

        public Guid OrganDonationSessionId { get; set; }

        public string Im1ConnectionToken { get; set; }

        public string UserSessionCreateReferenceCode { get; set; }

        public string NhsNumber { get; set; }

        public override TResult Accept<TResult>(IUserSessionVisitor<TResult> visitor)
            => visitor.Visit(this);
    }
}