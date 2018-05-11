namespace NHSOnline.Backend.Worker
{
    public static class Constants
    {
        public class ClaimTypes
        {
            public const string SessionId = "SessionId";
        }

        public class Cookies
        {
            public const string SessionId = "NHSO-Session-Id";
        }

        public class Headers
        {
            public const string NhsoPrefix = "NHSO-";
            public const string ConnectionToken = NhsoPrefix + "Connection-Token";
            public const string OdsCode = NhsoPrefix + "ODS-Code";
        }

        public class HttpContextItems
        {
            public const string UserSession = "UserSession";
        }
    }
}