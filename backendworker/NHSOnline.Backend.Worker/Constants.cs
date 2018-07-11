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

        public class CustomHttpStatusCodes
        {
            public const int Status460LimitReached = 460;
        }
        public class Headers
        {
            public const string NhsoPrefix = "NHSO-";
            public const string ConnectionToken = NhsoPrefix + "Connection-Token";
            public const string OdsCode = NhsoPrefix + "ODS-Code";
            public const string NhsNumber = NhsoPrefix + "Nhs-Number";
        }

        public class HttpContextItems
        {
            public const string UserSession = "UserSession";
        }

        public class OdsCodeFormats
        {
            public const string GpPracticeEnglandWales = @"^[A-Z0-9]{6}$";
        }
    }
}