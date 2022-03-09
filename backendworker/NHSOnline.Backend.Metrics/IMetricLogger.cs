using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics
{
    public interface IMetricLogger
    {
        Task AppointmentBook(AppointmentData data);

        Task AppointmentCancel(AppointmentData data);

        Task GpSessionCreated(LoginData data);

        Task Login(LoginData data);

        Task MedicalRecordView(MedicalRecordData data);

        Task MessageLinkClicked(MessageLinkClickedData data);

        Task MessageRead(MessageReadData data);

        Task NominatedPharmacyCreate(NominatedPharmacyData data);

        Task NominatedPharmacyUpdate(NominatedPharmacyData data);

        Task NotificationsDisabled();

        Task NotificationsEnabled();

        Task NotificationsPrompt(NotificationsPromptData data);

        Task OrganDonationCreateRegistration(OrganDonationData data);

        Task OrganDonationGetRegistration(OrganDonationData data);

        Task OrganDonationUpdateRegistration(OrganDonationData data);

        Task OrganDonationWithdrawRegistration(OrganDonationData data);

        Task RepeatPrescriptionOrder(RepeatPrescriptionData data);

        Task SilverIntegrationJumpOff(SilverIntegrationData data);

        Task SilverIntegrationJumpOffBlocked(SilverIntegrationJumpOffBlockedData data);

        Task TermsAndConditionsInitialConsent();

        Task UpliftStarted(UpliftStartedData data);

        Task UserResearchOptIn();

        Task UserResearchOptOut();
    }
}