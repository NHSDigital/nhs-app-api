namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class Dev18NhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme => "https";
        public string BaseHost => "dev18.signin.nhs.uk";
        public string AuthHost => "auth.dev18.signin.nhs.uk";
        public string UafHost => "uaf.dev18.signin.nhs.uk";
        public int Port => 443;
        public string AuthorizePath => "authorize";
    }
}