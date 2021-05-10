namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class NhsAppNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme => "https";
        public string BaseHost => "nhsapp.signin.nhs.uk";
        public string AuthHost => "auth.nhsapp.signin.nhs.uk";
        public string UafHost => "uaf.nhsapp.signin.nhs.uk";
        public int Port => 443;
        public string AuthorizePath => "authorize";
    }
}