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

        public class AuditingTitles
        {
            public const string ViewAppointmentAuditTypeRequest = "Appointments_ViewBooked_Request";
            public const string ViewAppointmentAuditTypeResponse = "Appointments_ViewBooked_Response";
            public const string BookAppointmentAuditTypeRequest = "Appointments_Book_Request";
            public const string BookAppointmentAuditTypeResponse = "Appointments_Book_Response";
            public const string CancelAppointmentAuditTypeRequest = "Appointments_Cancel_Request";
            public const string CancelAppointmentAuditTypeResponse = "Appointments_Cancel_Response";
            public const string GetSlotsAuditTypeRequest = "Appointments_GetSlots_Request";
            public const string GetSlotsAuditTypeResponse = "Appointments_GetSlots_Response";
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
