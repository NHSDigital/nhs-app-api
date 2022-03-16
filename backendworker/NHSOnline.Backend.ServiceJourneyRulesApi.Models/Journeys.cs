using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Journeys : ICloneable<Journeys>
    {
        public Appointments Appointments { get; set; }

        public Cdss CdssAdmin { get; set; }

        public Cdss CdssAdvice { get; set; }

        public bool? CoronavirusInformation { get; set; }

        public bool? Documents { get; set; }

        public HomeScreen HomeScreen { get; set; }

        public Im1Messaging Im1Messaging { get; set; }

        public MedicalRecord MedicalRecord { get; set; }

        public bool? Messaging { get; set; }

        public bool? Ndop { get; set; }

        public bool? NominatedPharmacy { get; set; }

        public bool? Notifications { get; set; }

        public bool? NotificationPrompt { get; set; }

        public bool? OneOneOne { get; set; }

        public Prescriptions Prescriptions { get; set; }

        public SilverIntegrations SilverIntegrations { get; set; }

        public Supplier? Supplier { get; set; }

        public bool? SupportsLinkedProfiles { get; set; }

        public bool? UserInfo { get; set; }

        public Wayfinder Wayfinder { get; set; }

        public Journeys Clone() => new Journeys()
        {
            Appointments = Appointments?.Clone(),
            CdssAdmin = CdssAdmin?.Clone(),
            CdssAdvice = CdssAdvice?.Clone(),
            CoronavirusInformation = CoronavirusInformation,
            Documents = Documents,
            HomeScreen = HomeScreen?.Clone(),
            Im1Messaging = Im1Messaging?.Clone(),
            MedicalRecord = MedicalRecord?.Clone(),
            Messaging = Messaging,
            Ndop = Ndop,
            NominatedPharmacy = NominatedPharmacy,
            Notifications = Notifications,
            NotificationPrompt = NotificationPrompt,
            OneOneOne = OneOneOne,
            Prescriptions = Prescriptions?.Clone(),
            SilverIntegrations = SilverIntegrations?.Clone(),
            SupportsLinkedProfiles = SupportsLinkedProfiles,
            UserInfo = UserInfo,
            Wayfinder = Wayfinder
        };

        public Journeys AddSupplier(Supplier? supplier)
        {
            Supplier = supplier;
            return this;
        }

        public void Merge(Journeys other)
        {
            if (other.Appointments?.Provider != null)
            {
                Appointments = other.Appointments;
            }

            if (other.CdssAdmin?.Provider != null)
            {
                CdssAdmin = other.CdssAdmin;
            }

            if (other.CdssAdvice?.Provider != null)
            {
                CdssAdvice = other.CdssAdvice;
            }

            if (other.CoronavirusInformation.HasValue)
            {
                CoronavirusInformation = other.CoronavirusInformation;
            }

            if(other.Documents.HasValue)
            {
                Documents = other.Documents;
            }

            if (other.HomeScreen != null)
            {
                HomeScreen = other.HomeScreen;
            }

            if (other.Im1Messaging != null)
            {
                Im1Messaging ??= new Im1Messaging();
                Im1Messaging.Merge(other.Im1Messaging);
            }

            if (other.MedicalRecord?.Provider != null)
            {
                MedicalRecord = other.MedicalRecord;
            }

            if (other.Messaging.HasValue)
            {
                Messaging = other.Messaging;
            }

            if (other.Ndop.HasValue)
            {
                Ndop = other.Ndop;
            }

            if (other.NominatedPharmacy.HasValue)
            {
                NominatedPharmacy = other.NominatedPharmacy;
            }

            if (other.Notifications.HasValue)
            {
                Notifications = other.Notifications;
            }

            if (other.NotificationPrompt.HasValue)
            {
                NotificationPrompt = other.NotificationPrompt;
            }

            if (other.OneOneOne.HasValue)
            {
                OneOneOne = other.OneOneOne;
            }

            if (other.Prescriptions?.Provider != null)
            {
                Prescriptions = other.Prescriptions;
            }

            if (other.SilverIntegrations != null)
            {
                SilverIntegrations ??= new SilverIntegrations();
                SilverIntegrations.Merge(other.SilverIntegrations);
            }

            if (other.SupportsLinkedProfiles.HasValue)
            {
                SupportsLinkedProfiles = other.SupportsLinkedProfiles;
            }

            if (other.UserInfo.HasValue)
            {
                UserInfo = other.UserInfo;
            }

            if (other.Wayfinder != null)
            {
                Wayfinder = other.Wayfinder;
            }
        }
    }
}