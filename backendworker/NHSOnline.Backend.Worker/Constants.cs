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
            public const int Status464OdsCodeNotSupportedOrNoNhsNumber = 464;
            public const int Status465FailedAgeRequirement = 465;
            public const int Status466MedicationAlreadyOrderedWithinLast30Days = 466;
        }

        public static class AppConfig
        {
            public const string GitCommitId = "Version:CommitId";
            public const string ThrottlingEnabled = "ThrottlingEnabled";
        }

        public static class EnvironmentalVariables
        {
            public const string VersionTag = "VERSION_TAG";
        }

        public static class HttpHeaders
        {
            private const string NhsoPrefix = "NHSO-";
            public const string ConnectionToken = NhsoPrefix + "Connection-Token";
            public const string OdsCode = NhsoPrefix + "ODS-Code";
            public const string NhsNumber = NhsoPrefix + "Nhs-Number";
            public const string Surname = NhsoPrefix + "Surname";
            public const string DateOfBirth = NhsoPrefix + "Date-Of-Birth";
            public const string IdentityToken = NhsoPrefix + "Identity-Token";
            public const string WebAppVersion = NhsoPrefix + "Web-Version-Tag";
            public const string NativeAppVersion = NhsoPrefix + "Native-Version-Tag";
        }

        public static class AuditingTitles
        {
            public const string Im1ConnectionVerifyResponse = "Im1Connection_Verify_Response";
            public const string Im1ConnectionRegisterResponse = "Im1Connection_Register_Response";
            public const string SessionCreateRequest = "Session_Create_Request";
            public const string SessionCreateResponse = "Session_Create_Response";
            public const string SessionDeleteRequest = "Session_Delete_Request";
            public const string SessionDeleteResponse = "Session_Delete_Response";
            public const string SessionExtendResponse = "Session_Extend_Response";
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
            public const string TermsAndConditionsRecordConsentAuditTypeRequest = "TermsAndConditions_RecordConsent_Request";
            public const string TermsAndConditionsRecordConsentAuditTypeResponse = "TermsAndConditions_RecordConsent_Response";
            public const string TermsAndConditionsAnalyticsCookieAcceptance = "TermsAndConditions_RecordAnalyticsCookie_Acceptance";
            public const string GetOrganDonationAuditTypeResponse = "OrganDonation_Get_Response";
            public const string GetOrganDonationAuditTypeRequest = "OrganDonation_Get_Request";
            public const string OrganDonationRegistrationAuditTypeResponse = "OrganDonation_Registration_Response";
            public const string OrganDonationRegistrationAuditTypeRequest = "OrganDonation_Registration_Request";
            public const string GetDemographicsAuditTypeRequest = "Demographics_Get_Request";
            public const string GetDemographicsAuditTypeResponse = "Demographics_Get_Response";
            public const string GetOrganDonationReferenceDataAuditTypeRequest = "OrganDonation_ReferenceData_Request";
            public const string GetOrganDonationReferenceDataAuditTypeResponse = "OrganDonation_ReferenceData_Response";
            public const string GetTestResultAuditTypeRequest = "TestResult_Get_Request";
            public const string GetTestResultAuditTypeResponse = "TestResult_Get_Response";
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
            public const string NhsNumber = "nhs_number";
        }

        public static class Regex
        {
            public const string GuidRegex = @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}";
        }

        public static class TppConstants
        {
            public const string RequestIdentifierHeader = "type";
        }
        
        public static class VisionConstants
        {
            public const string RequestIdentifierHeader = "type";
        }

        public static class SupportedDeviceNames
        {
            public const string Android = "Android";
            public const string iOS = "iOS";
        }

        public static class OrganDonationConstants
        {
            public const string AllOrgansChoiceKey = "all";
            public const string YesChoiceValue = "yes";
            public const string NoChoiceValue = "no";
            public const string NotStatedChoiceValue = "not-stated";
            public const string DateFormat = "yyyy-MM-dd";
        }
    }
}
