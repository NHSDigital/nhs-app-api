using Microsoft.AspNetCore.Http;

namespace NHSOnline.HttpMocks.CitizenId.Models
{
    public class PatientUpliftModel
    {
        public string AuthHostName { get; set; } = string.Empty;

        public string PatientId { get; set; } = string.Empty;

        public string? PatientName { get; set; } = string.Empty;

        public string Redirect { get; set; } = string.Empty;

        public string Scope { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public HttpRequest? Request { get; set; }
    }

    public enum UpliftSubmissionType
    {
        Success,
        TermsAndConditionsDeclined,
        Error,
    }
}