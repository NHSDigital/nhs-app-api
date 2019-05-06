using System.Net;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public class GetNominatedPharmacyResult
    {
        public HttpStatusCode HttpStatusCode { get; }

        public string PharmacyOdsCode { get; }
        
        public string NominatedPharmacyType { get; }

        public string PertinentSerialChangeNumber { get; }

        public bool HasValidPharmacyType { get; }

        public GetNominatedPharmacyResult(HttpStatusCode httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public GetNominatedPharmacyResult(HttpStatusCode httpStatusCode, string pharmacyOdsCode, string pertinentSerialChangeNumber, bool hasValidPharmacyType, string nominatedPharmacyType) : this(httpStatusCode)
        {
            PharmacyOdsCode = pharmacyOdsCode;
            NominatedPharmacyType = nominatedPharmacyType;
            PertinentSerialChangeNumber = pertinentSerialChangeNumber;
            HasValidPharmacyType = hasValidPharmacyType;
        }
    }
}
