using System;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Journeys : ICloneable<Journeys>
    {
        public Appointments Appointments { get; set; }

        public Cdss CdssAdvice { get; set; }

        public Cdss CdssAdmin { get; set; }

        public MedicalRecord MedicalRecord { get; set; }

        public Prescriptions Prescriptions { get; set; }

        public bool? NominatedPharmacy { get; set; }

        public bool? Notifications { get; set; }

        public bool? Messaging { get; set; }

        public Supplier Supplier { get; set; }

        public Journeys Clone() => new Journeys
        {
            Appointments = Appointments?.Clone(),
            CdssAdvice = CdssAdvice?.Clone(),
            CdssAdmin = CdssAdmin?.Clone(),
            MedicalRecord = MedicalRecord?.Clone(),
            Prescriptions = Prescriptions?.Clone(),
            NominatedPharmacy = NominatedPharmacy,
            Notifications = Notifications,
            Messaging = Messaging
        };

        public Journeys AddSupplier(Supplier supplier)
        {
            Supplier = supplier;
            return this;
        }
    }
}