using NHSOnline.Backend.NominatedPharmacy.Soap;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class PharmacyCheck
    {
        public bool IsValid { get; set; }
        
        public NominatedPharmacyTypes.PatientCareProvisionEvent PatientCareProvisionEvent { get; set; }

        public string PharmacyType { get; set; }
    }
}