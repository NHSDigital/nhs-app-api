using System;

namespace NHSOnline.Backend.Support.Session
{
    public sealed class P9UserSession: P5UserSession
    {
        private P9UserSession()
        { }

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

        public string NhsNumber { get; set; }

        public override TResult Accept<TResult>(IUserSessionVisitor<TResult> visitor)
            => visitor.Visit(this);
    }
}