namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class SandpitNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme { get; } = "https";
        public string BaseHost { get; } = "sandpit.signin.nhs.uk";
        public string AuthHost { get; } = "auth.sandpit.signin.nhs.uk";
        public string UafHost { get; } = "uaf.sandpit.signin.nhs.uk";
        public int Port { get; } = 443;
        public string AuthorizePath { get; } = "authorize";
    }
}