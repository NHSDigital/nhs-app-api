using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics
{
    public interface IMetricLogger
    {
        Task AppointmentBook(AppointmentData data);

        Task AppointmentCancel(AppointmentData data);

        Task Login(LoginData data);

        Task UpliftStarted(UpliftStartedData data);

        Task UserResearchOptOut();

        Task UserResearchOptIn();

        Task TermsAndConditionsInitialConsent();

        Task MessageRead(MessageReadData data);

        Task NotificationsEnabled();

        Task NotificationsDisabled();

        Task OrganDonationWithdrawRegistration(OrganDonationData data);

        Task OrganDonationGetRegistration(OrganDonationData data);

        Task OrganDonationCreateRegistration(OrganDonationData data);

        Task OrganDonationUpdateRegistration(OrganDonationData data);

        Task NotificationsPrompt(NotificationsPromptData data);

        Task SilverIntegrationJumpOff(SilverIntegrationData data);

        Task MedicalRecordView(MedicalRecordData data);

        Task NominatedPharmacyCreate(NominatedPharmacyData data);

        Task NominatedPharmacyUpdate(NominatedPharmacyData data);

        Task RepeatPrescriptionOrder(RepeatPrescriptionData data);
    }
}
