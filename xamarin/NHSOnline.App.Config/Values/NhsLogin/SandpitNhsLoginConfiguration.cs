namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class SandpitNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme => "https";
        public string BaseHost => "sandpit.signin.nhs.uk";
        public string AuthHost => "auth.sandpit.signin.nhs.uk";
        public string UafHost => "uaf.sandpit.signin.nhs.uk";
        public int Port => 443;
        public string AuthorizePath => "authorize";
    }
}