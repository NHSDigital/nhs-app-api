using System.Net;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public class UpdateNominatedPharmacyResult
    {
        public HttpStatusCode HttpStatusCode { get; }

        public UpdateNominatedPharmacyResult(HttpStatusCode httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}