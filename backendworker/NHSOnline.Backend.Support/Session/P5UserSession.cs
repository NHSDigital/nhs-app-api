namespace NHSOnline.Backend.Support.Session
{
    public class P5UserSession: UserSession
    {
        public string CsrfToken { get; set; }

        public CitizenIdUserSession CitizenIdUserSession { get; set; }

        public override TResult Accept<TResult>(IUserSessionVisitor<TResult> visitor)
            => visitor.Visit(this);
    }
}