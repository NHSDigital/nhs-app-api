namespace NHSOnline.Backend.Support.Session
{
    public abstract class UserSession
    {
        protected UserSession() { }

        protected UserSession(string csrfToken) => CsrfToken = csrfToken;

        public string Key { get; set; }
        public string CsrfToken { get; set; }

        public abstract TResult Accept<TResult>(IUserSessionVisitor<TResult> visitor);
    }
}