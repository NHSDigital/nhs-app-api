using System.Net;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public class GetNominatedPharmacyResult
    {
        public HttpStatusCode HttpStatusCode { get; }

        public string PharmacyOdsCode { get; }
        
        public string NominatedPharmacyType { get; }

        public string PertinentSerialChangeNumber { get; }

        public GetNominatedPharmacyResult(HttpStatusCode httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public GetNominatedPharmacyResult(HttpStatusCode httpStatusCode, string pharmacyOdsCode, string nominatedPharmacyType, string pertinentSerialChangeNumber) : this(httpStatusCode)
        {
            PharmacyOdsCode = pharmacyOdsCode;
            NominatedPharmacyType = nominatedPharmacyType;
            PertinentSerialChangeNumber = pertinentSerialChangeNumber;
        }
    }
}
