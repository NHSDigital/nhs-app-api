using System;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.CitizenId
{
    public sealed class CitizenIdSessionResult: IAuditedResult
    {
        public int StatusCode { get; set; }
        public string Im1ConnectionToken { get; set; }
        public string NhsNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public CitizenIdUserSession Session { get; set; }

        string IAuditedResult.Details => $"Created Citizen Id Session {StatusCode}";
    }
}