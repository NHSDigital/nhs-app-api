namespace NHSOnline.IntegrationTests.WebIntegration.Pkb
{
    public static class PhrPath
    {
        public const string CarePlans = "/auth/listPlans.action";
        public const string HospitalAndOtherPrescriptions = "/auth/manageMedications.action?tab=treatments";
        public const string MessagesAndOnlineConsultations = "/auth/getInbox.action?tab=messages";
        public const string RecordSharing = "/patient/myConsentTeam.action?tab=invitations";
        public const string SharedHealthLinks = "/library/manageLibrary.action";
        public const string TestResults = "/test/myTests.action";
        public const string TrackYourHealth = "/pkbNhsMenu.action";
        public const string ViewAppointments = "/diary/listAppointments.action";
    }
}