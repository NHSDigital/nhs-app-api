namespace NHSOnline.Backend.Support.Session
{
    public abstract class UserSession
    {
        public string Key { get; set; }

        public abstract TResult Accept<TResult>(IUserSessionVisitor<TResult> visitor);
    }
}