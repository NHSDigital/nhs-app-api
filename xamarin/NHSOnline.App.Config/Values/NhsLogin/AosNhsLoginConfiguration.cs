namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class AosNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme => "https";
        public string BaseHost => "aos.signin.nhs.uk";
        public string AuthHost => "auth.aos.signin.nhs.uk";
        public string UafHost => "uaf.aos.signin.nhs.uk";
        public int Port => 443;
        public string AuthorizePath => "authorize";
    }
}