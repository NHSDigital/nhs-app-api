using System;

namespace NHSOnline.Backend.Support.Session
{
    public class P5UserSession: UserSession
    {
        [Obsolete("Public for MVC model binding")]
        public P5UserSession()
        { }

        public P5UserSession(string csrfToken, CitizenIdUserSession citizenIdUserSession)
        {
            CsrfToken = csrfToken;
            CitizenIdUserSession = citizenIdUserSession;
        }

        public string CsrfToken { get; set; }

        public CitizenIdUserSession CitizenIdUserSession { get; set; }

        public override TResult Accept<TResult>(IUserSessionVisitor<TResult> visitor)
            => visitor.Visit(this);
    }
}