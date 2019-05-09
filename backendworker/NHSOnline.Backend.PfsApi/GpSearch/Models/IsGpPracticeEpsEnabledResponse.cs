using System.Net;

namespace NHSOnline.Backend.Worker.GpSearch.Models.Pharmacy
{
    public class IsGpPracticeEpsEnabledResponse
    {
        public IsGpPracticeEpsEnabledResponse(HttpStatusCode statusCode, bool isGpEpsEnabled)
        {
            HttpStatusCode = statusCode;
            IsGpEpsEnabled = isGpEpsEnabled;
        }

        public IsGpPracticeEpsEnabledResponse(HttpStatusCode statusCode)
        {
            HttpStatusCode = statusCode;
        }

        public HttpStatusCode HttpStatusCode { get; }

        public bool IsGpEpsEnabled { get; }

    }
}