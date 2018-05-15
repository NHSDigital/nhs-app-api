using System;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Appointments
{
    public class SlotsMetadataGetQueryParameters
    {
        public string UserPatientLinkToken { get; set; }
        public DateTimeOffset SessionStartDate { get; set; }
        public DateTimeOffset SessionEndDate { get; set; }
    }
}