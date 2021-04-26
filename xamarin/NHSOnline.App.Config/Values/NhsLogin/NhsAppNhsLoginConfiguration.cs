namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class NhsAppNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme { get; } = "https";
        public string BaseHost { get; } = "nhsapp.signin.nhs.uk";
        public string AuthHost { get; } = "auth.nhsapp.signin.nhs.uk";
        public string UafHost { get; } = "uaf.nhsapp.signin.nhs.uk";
        public int Port { get; } = 443;
        public string AuthorizePath { get; } = "authorize";
    }
}