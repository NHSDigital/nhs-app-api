using NHSOnline.Backend.Support;
using System;

namespace NHSOnline.Backend.PfsApi.CitizenId
{
    public sealed class CitizenIdSessionResult
    {
        public int StatusCode { get; set; }
        public string Im1ConnectionToken { get; set; }
        public string NhsNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public CitizenIdUserSession Session { get; set; }
    }
}