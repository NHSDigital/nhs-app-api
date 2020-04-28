using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Journeys : ICloneable<Journeys>
    {
        public HomeScreen HomeScreen { get; set; }

        public Appointments Appointments { get; set; }

        public Cdss CdssAdvice { get; set; }

        public Cdss CdssAdmin { get; set; }

        public MedicalRecord MedicalRecord { get; set; }

        public Prescriptions Prescriptions { get; set; }

        public bool? NominatedPharmacy { get; set; }

        public bool? Notifications { get; set; }

        public bool? Messaging { get; set; }

        public Supplier Supplier { get; set; }

        public SilverIntegrations SilverIntegrations { get; set; }

        public bool? UserInfo { get; set; }

        public bool? Documents { get; set; }

        public Im1Messaging Im1Messaging { get; set; }

        public Journeys Clone() => new Journeys
        {
            HomeScreen = HomeScreen?.Clone(),
            Appointments = Appointments?.Clone(),
            CdssAdvice = CdssAdvice?.Clone(),
            CdssAdmin = CdssAdmin?.Clone(),
            MedicalRecord = MedicalRecord?.Clone(),
            Prescriptions = Prescriptions?.Clone(),
            NominatedPharmacy = NominatedPharmacy,
            Notifications = Notifications,
            Messaging = Messaging,
            UserInfo = UserInfo,
            SilverIntegrations = SilverIntegrations?.Clone(),
            Documents = Documents,
            Im1Messaging = Im1Messaging?.Clone(),
        };

        public Journeys AddSupplier(Supplier supplier)
        {
            Supplier = supplier;
            return this;
        }

        public void Merge(Journeys other)
        {
            if (other.HomeScreen != null)
            {
                HomeScreen = other.HomeScreen;
            }

            if (other.Appointments?.Provider != null)
            {
                Appointments = other.Appointments;
            }

            if (other.CdssAdvice?.Provider != null)
            {
                CdssAdvice = other.CdssAdvice;
            }

            if (other.CdssAdmin?.Provider != null)
            {
                CdssAdmin = other.CdssAdmin;
            }

            if (other.MedicalRecord?.Provider != null)
            {
                MedicalRecord = other.MedicalRecord;
            }

            if (other.Prescriptions?.Provider != null)
            {
                Prescriptions = other.Prescriptions;
            }

            if (other.NominatedPharmacy.HasValue)
            {
                NominatedPharmacy = other.NominatedPharmacy;
            }

            if (other.Notifications.HasValue)
            {
                Notifications = other.Notifications;
            }

            if (other.Messaging.HasValue)
            {
                Messaging = other.Messaging;
            }

            if (other.UserInfo.HasValue)
            {
                UserInfo = other.UserInfo;
            }

            if (other.Documents.HasValue)
            {
                Documents = other.Documents;
            }

            if (other.SilverIntegrations != null)
            {
                SilverIntegrations ??= new SilverIntegrations();
                SilverIntegrations.Merge(other.SilverIntegrations);
            }

            if (other.Im1Messaging != null)
            {
                Im1Messaging ??= new Im1Messaging();
                Im1Messaging.Merge(other.Im1Messaging);
            }
        }
    }
}