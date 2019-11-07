using System.Net;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public class GetNominatedPharmacyResponse
    {
        public HttpStatusCode HttpStatusCode { get; }

        public string PharmacyOdsCode { get; }
        
        public string NominatedPharmacyType { get; }

        public string PertinentSerialChangeNumber { get; }

        public bool HaveAllChecksPassed { get; }

        public GetNominatedPharmacyResponse(HttpStatusCode httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public GetNominatedPharmacyResponse(HttpStatusCode httpStatusCode, bool haveAllChecksPassed) : this(httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
            HaveAllChecksPassed = haveAllChecksPassed;
        }
        
        public GetNominatedPharmacyResponse(HttpStatusCode httpStatusCode, string pharmacyOdsCode, string pertinentSerialChangeNumber, bool haveAllChecksPassed, string nominatedPharmacyType) : this(httpStatusCode)
        {
            PharmacyOdsCode = pharmacyOdsCode;
            NominatedPharmacyType = nominatedPharmacyType;
            PertinentSerialChangeNumber = pertinentSerialChangeNumber;
            HaveAllChecksPassed = haveAllChecksPassed;
        }
    }
}
