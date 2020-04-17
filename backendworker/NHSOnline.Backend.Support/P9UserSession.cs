using System;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.Support
{
    public sealed class P9UserSession: P5UserSession
    {
        [Obsolete("Public for MVC model binding")]
        public P9UserSession() { }

        public P9UserSession(
            string csrfToken,
            CitizenIdUserSession citizenIdUserSession,
            GpUserSession gpUserSession,
            string im1ConnectionToken) : base(csrfToken, citizenIdUserSession)
        {
            GpUserSession = gpUserSession;
            Im1ConnectionToken = im1ConnectionToken;
            OrganDonationSessionId = Guid.NewGuid();
        }

        public GpUserSession GpUserSession { get; set; }

        public Guid OrganDonationSessionId { get; set; }

        public string Im1ConnectionToken { get; set; }

        public override TResult Accept<TResult>(IUserSessionVisitor<TResult> visitor)
            => visitor.Visit(this);
    }
}