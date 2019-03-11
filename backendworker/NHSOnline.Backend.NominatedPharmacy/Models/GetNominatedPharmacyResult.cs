using System.Net;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public class GetNominatedPharmacyResult
    {
        public HttpStatusCode HttpStatusCode { get; }

        public string PharmacyOdsCode { get; }

        public GetNominatedPharmacyResult(HttpStatusCode httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public GetNominatedPharmacyResult(HttpStatusCode httpStatusCode, string pharmacyOdsCode) : this(httpStatusCode)
        {
            PharmacyOdsCode = pharmacyOdsCode;
        }
    }
}
