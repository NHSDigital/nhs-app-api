namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class AosNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme { get; } = "https";
        public string BaseHost { get; } = "aos.signin.nhs.uk";
        public string AuthHost { get; } = "auth.aos.signin.nhs.uk";
        public string UafHost { get; } = "uaf.aos.signin.nhs.uk";
        public int Port { get; } = 443;
        public string AuthorizePath { get; } = "authorize";
    }
}