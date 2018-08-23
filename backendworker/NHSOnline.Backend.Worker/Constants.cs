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
            public const int Status461TooLate = 461;
            public const int Status462FailedToRecordConsent = 462;
            public const int Status463FailedToFetchConsent = 463;
        }
        
        public static class HttpHeaders
        {
            private const string NhsoPrefix = "NHSO-";
            public const string ConnectionToken = NhsoPrefix + "Connection-Token";
            public const string OdsCode = NhsoPrefix + "ODS-Code";
            public const string NhsNumber = NhsoPrefix + "Nhs-Number";
            public const string IdentityToken = NhsoPrefix + "Identity-Token";
        }

        public static class AuditingTitles
        {
            public const string Im1ConnectionVerifyResponse = "Im1Connection_Verify_Response";
            public const string Im1ConnectionRegisterResponse = "Im1Connection_Register_Response";
            public const string SessionCreateResponse = "Session_Create_Response";
            public const string SessionDeleteRequest = "Session_Delete_Request";
            public const string SessionDeleteResponse = "Session_Delete_Response";
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
            public const string RepeatPrescriptionsViewHistoryRequest = "RepeatPrescriptions_ViewHistory_Request";
            public const string RepeatPrescriptionsViewHistoryResponse = "RepeatPrescriptions_ViewHistory_Response";
            public const string RepeatPrescriptionsViewRepeatMedicationsRequest = "RepeatPrescriptions_ViewRepeatMedications_Request";
            public const string RepeatPrescriptionsViewRepeatMedicationsResponse = "RepeatPrescriptions_ViewRepeatMedications_Response";
            public const string RepeatPrescriptionsOrderRepeatMedicationsRequest = "RepeatPrescriptions_OrderRepeatMedications_Request";
            public const string RepeatPrescriptionsOrderRepeatMedicationsResponse = "RepeatPrescriptions_OrderRepeatMedications_Response";
            public const string GetLinkageDetailsAuditTypeRequest = "Linkage_GetDetails_Request";
            public const string GetLinkageDetailsAuditTypeResponse = "Linkage_GetDetails_Response";
            public const string CreateLinkageKeyAuditTypeRequest = "Linkage_CreateKey_Request";
            public const string CreateLinkageKeyAuditTypeResponse = "Linkage_CreateKey_Response";
            public const string GetNdopTokenAuditTypeRequest = "Ndop_GetToken_Request";
            //Retrieves a patient's Linkage details
            public const string TermsAndConditionsRecordConsentAuditTypeRequest = "TermsAndConditions_RecordConsent_Request";
            public const string TermsAndConditionsRecordConsentAuditTypeResponse = "TermsAndConditions_RecordConsent_Response";

        }

        public static class HttpContextItems
        {
            public const string UserSession = "UserSession";
        }

        public static class OdsCodeFormats
        {
            public const string GpPracticeEnglandWales = @"^[A-Z0-9]{6}$";
        }

        public static class CitizenIdClaimTypes
        {
            public const string Im1ConnectionTokenClaim = "im1_token";
            public const string OdscodeClaim = "ods_code";
        }
    }
}
