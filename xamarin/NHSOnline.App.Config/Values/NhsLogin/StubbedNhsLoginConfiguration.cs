namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal sealed class StubbedNhsLoginConfiguration : INhsLoginConfiguration
    {
        public string Scheme => "http";
        public string BaseHost => "nhslogin.stubs.local.bitraft.io";
        public string AuthHost => "auth.nhslogin.stubs.local.bitraft.io";
        public string UafHost => "uaf.nhslogin.stubs.local.bitraft.io";
        public int Port => 8080;
        public string AuthorizePath => "/citizenid/authorize";
    }
}