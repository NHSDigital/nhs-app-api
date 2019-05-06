using NHSOnline.Backend.NominatedPharmacy.Soap;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public struct PharmacyCheck
    {
        public bool IsValid { get; set; }
        
        public NominatedPharmacyTypes.PatientCareProvisionEvent PatientCareProvisionEvent { get; set; }

        public string PharmacyType { get; set; }
    }
}