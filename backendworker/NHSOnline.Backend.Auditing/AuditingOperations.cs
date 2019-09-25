namespace NHSOnline.Backend.Auditing
{
    public static class AuditingOperations
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
        public const string ViewPatientRecordSectionAuditTypeRequest = "PatientRecord_Section_View_Request";
        public const string ViewPatientRecordAuditTypeResponse = "PatientRecord_View_Response";
        public const string ViewPatientRecordSectionAuditTypeResponse = "PatientRecord_Section_View_Response";
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
        public const string OrganDonationUpdateAuditTypeResponse = "OrganDonation_Update_Response";
        public const string OrganDonationUpdateAuditTypeRequest = "OrganDonation_Update_Request";
        public const string OrganDonationWithdrawAuditTypeRequest = "OrganDonation_Withdraw_Request";
        public const string OrganDonationWithdrawAuditTypeResponse = "OrganDonation_Withdraw_Response";
        public const string GetDemographicsAuditTypeRequest = "Demographics_Get_Request";
        public const string GetDemographicsAuditTypeResponse = "Demographics_Get_Response";
        public const string GetOrganDonationReferenceDataAuditTypeRequest = "OrganDonation_ReferenceData_Request";
        public const string GetOrganDonationReferenceDataAuditTypeResponse = "OrganDonation_ReferenceData_Response";
        public const string GetTestResultAuditTypeRequest = "TestResult_Get_Request";
        public const string GetTestResultAuditTypeResponse = "TestResult_Get_Response";
        public const string GetDocumentAuditTypeRequest = "Documents_Get_Request";
        public const string GetDocumentAuditTypeResponse = "Documents_Get_Response";
        public const string GetNominatedPharmacy = "NominatedPharmacy_Get_Response";
        public const string UpdatedNominatedPharmacy = "NominatedPharmacy_Update_Response";
        public const string SearchNominatedPharmacyAuditTypeResponse = "SearchNominatedPharmacy_Get_Response";
        public const string GetServiceJourneyRulesAuditTypeRequest = "ServiceJourneyRules_Get_Request";
        public const string OnlineConsultationsDemographicAuditTypeRequest = "OnlineConsultations_Demographics_Request";
        public const string RegisterUsersDeviceAuditTypeRequest = "Users_Device_Registration_Request";
        public const string RegisterUsersDeviceAuditTypeResponse = "Users_Device_Registration_Response";
        public const string GetUserMessagesAuditTypeRequest = "Users_Messages_Get_Request";
        public const string GetUserMessagesAuditTypeResponse = "Users_Messages_Get_Response";
    }
}