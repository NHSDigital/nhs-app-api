namespace NHSOnline.App.Config.Values.Staging
{
    internal sealed class StagingNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme { get; } = "https";
        public string BaseHost { get; } = "ext.signin.nhs.uk";
        public string AuthHost { get; } = "auth.ext.signin.nhs.uk";
        public string UafHost { get; } = "uaf.ext.signin.nhs.uk";
        public int Port { get; } = 443;
        public string AuthorizePath { get; } = "authorize";
    }
}