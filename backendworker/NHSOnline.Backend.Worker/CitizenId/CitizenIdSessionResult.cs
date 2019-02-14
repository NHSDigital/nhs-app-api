using NHSOnline.Backend.Support;
using System;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public class CitizenIdSessionResult
    {
        public int StatusCode { get; set; }
        public string OdsCode { get; set; }
        public string Im1ConnectionToken { get; set; }
        public string NhsNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public CitizenIdUserSession Session { get; set; }
    }
}