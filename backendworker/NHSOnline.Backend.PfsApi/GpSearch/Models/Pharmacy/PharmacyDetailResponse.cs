
using System.Net;

namespace NHSOnline.Backend.Worker.GpSearch.Models.Pharmacy
{
    public class PharmacyDetailResponse
    {
        public PharmacyDetailResponse(HttpStatusCode statusCode, Organisation pharmacy = null)
        {
            this.StatusCode = statusCode;
            this.Pharmacy = pharmacy;
        }

        public HttpStatusCode StatusCode { get; set; }
        public Organisation Pharmacy { get; set; }
    }
}