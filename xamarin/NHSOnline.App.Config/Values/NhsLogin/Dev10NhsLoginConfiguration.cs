namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class Dev10NhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme { get; } = "https";
        public string BaseHost { get; } = "dev10.signin.nhs.uk";
        public string AuthHost { get; } = "auth.dev10.signin.nhs.uk";
        public string UafHost { get; } = "uaf.dev10.signin.nhs.uk";
        public int Port { get; } = 443;
        public string AuthorizePath { get; } = "authorize";
    }
}