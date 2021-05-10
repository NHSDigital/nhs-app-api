namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class ExtNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme => "https";
        public string BaseHost => "ext.signin.nhs.uk";
        public string AuthHost => "auth.ext.signin.nhs.uk";
        public string UafHost => "uaf.ext.signin.nhs.uk";
        public int Port => 443;
        public string AuthorizePath => "authorize";
    }
}