using Newtonsoft.Json;

namespace NHSOnline.Backend.Support.Session
{
    public class P5UserSession: UserSession
    {
        protected P5UserSession()
        { }

        public P5UserSession(string csrfToken, CitizenIdUserSession citizenIdUserSession)
            : base(csrfToken)
        {
            CitizenIdUserSession = citizenIdUserSession;
        }

        public CitizenIdUserSession CitizenIdUserSession { get; set; }

        [JsonIgnore]
        public virtual string OdsCode => CitizenIdUserSession.OdsCode;

        public override TResult Accept<TResult>(IUserSessionVisitor<TResult> visitor)
            => visitor.Visit(this);
    }
}