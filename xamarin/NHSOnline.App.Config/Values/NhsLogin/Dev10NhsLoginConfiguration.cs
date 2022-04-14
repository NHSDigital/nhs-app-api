namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class Dev10NhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme => "https";
        public string BaseHost => "dev10.signin.nhs.uk";
        public string AuthHost => "auth.dev10.signin.nhs.uk";
        public string UafHost => "uaf.dev10.signin.nhs.uk";
        public int Port => 443;
        public string AuthorizePath => "authorize";
    }
}