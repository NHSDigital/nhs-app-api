using System;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Appointments
{
    public class SlotsGetQueryParameters
    {
        public string UserPatientLinkToken { get; set; }
        public DateTimeOffset FromDateTime { get; set; }
        public DateTimeOffset ToDateTime { get; set; }
    }
}