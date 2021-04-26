namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class ExtNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme { get; } = "https";
        public string BaseHost { get; } = "ext.signin.nhs.uk";
        public string AuthHost { get; } = "auth.ext.signin.nhs.uk";
        public string UafHost { get; } = "uaf.ext.signin.nhs.uk";
        public int Port { get; } = 443;
        public string AuthorizePath { get; } = "authorize";
    }
}