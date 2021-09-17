namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class ProductionNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme => "https";
        public string BaseHost => "login.nhs.uk";
        public string AuthHost => "auth.login.nhs.uk";
        public string UafHost => "uaf.login.nhs.uk";
        public int Port => 443;
        public string AuthorizePath => "authorize";
    }
}