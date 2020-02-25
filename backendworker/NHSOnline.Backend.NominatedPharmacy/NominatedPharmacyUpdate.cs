namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyUpdate
    {
        public string NhsNumber { get; set; }

        public string UpdatedOdsCode { get; set; }
        
        public string ObjectId { get; set; }

        public string PertinentSerialChangeNumber { get; set; }

        public bool HasExistingNominatedPharmacy { get; set; }
    }
}
