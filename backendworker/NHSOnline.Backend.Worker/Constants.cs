namespace NHSOnline.Backend.Worker
{
    public static class Constants
    {
        public static class ClaimTypes
        {
            public const string SessionId = "SessionId";
        }

        public static class CookieNames
        {
            public const string SessionId = "NHSO-Session-Id";
        }

        public static class CustomHttpStatusCodes
        {
            public const int Status460LimitReached = 460;
        }
        
        public static class HttpHeaders
        {
            private const string NhsoPrefix = "NHSO-";
            public const string ConnectionToken = NhsoPrefix + "Connection-Token";
            public const string OdsCode = NhsoPrefix + "ODS-Code";
            public const string NhsNumber = NhsoPrefix + "Nhs-Number";
        }

        public static class AuditingTitles
        {
            public const string Im1ConnectionVerifyResponse = "Im1Connection_Verify_Response";
            public const string Im1ConnectionRegisterResponse = "Im1Connection_Register_Response";
            public const string SessionCreateResponse = "Session_Create_Response";
            public const string ViewAppointmentAuditTypeRequest = "Appointments_ViewBooked_Request";
            public const string ViewAppointmentAuditTypeResponse = "Appointments_ViewBooked_Response";
            public const string BookAppointmentAuditTypeRequest = "Appointments_Book_Request";
            public const string BookAppointmentAuditTypeResponse = "Appointments_Book_Response";
            public const string CancelAppointmentAuditTypeRequest = "Appointments_Cancel_Request";
            public const string CancelAppointmentAuditTypeResponse = "Appointments_Cancel_Response";
            public const string GetSlotsAuditTypeRequest = "Appointments_GetSlots_Request";
            public const string GetSlotsAuditTypeResponse = "Appointments_GetSlots_Response";
            public const string ViewPatientRecordAuditTypeRequest = "PatientRecord_View_Request";
            public const string ViewPatientRecordAuditTypeResponse = "PatientRecord_View_Response";
        }

        public static class HttpContextItems
        {
            public const string UserSession = "UserSession";
        }

        public static class OdsCodeFormats
        {
            public const string GpPracticeEnglandWales = @"^[A-Z0-9]{6}$";
        }
    }
}
