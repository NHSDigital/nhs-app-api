using System.Net;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public class GetNominatedPharmacyResult
    {
        public HttpStatusCode HttpStatusCode { get; }

        public string PharmacyOdsCode { get; }

        public string PertinentSerialChangeNumber { get; }

        public GetNominatedPharmacyResult(HttpStatusCode httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public GetNominatedPharmacyResult(HttpStatusCode httpStatusCode, string pharmacyOdsCode, string pertinentSerialChangeNumber) : this(httpStatusCode)
        {
            PharmacyOdsCode = pharmacyOdsCode;
            PertinentSerialChangeNumber = pertinentSerialChangeNumber;
        }
    }
}
