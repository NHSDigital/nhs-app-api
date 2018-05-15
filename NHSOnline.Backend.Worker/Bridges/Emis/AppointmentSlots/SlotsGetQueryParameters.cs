using System;

namespace NHSOnline.Backend.Worker.Bridges.Emis.AppointmentSlots
{
    public class SlotsGetQueryParameters
    {
        public string UserPatientLinkToken { get; set; }
        public DateTimeOffset FromDateTime { get; set; }
        public DateTimeOffset ToDateTime { get; set; }
    }
}