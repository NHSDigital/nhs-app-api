using System;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models.PatientRecord
{
    public class AllergyResponse
    {
        public string Term { get; set; }
        public DateTimeOffset AvailabilityDateTime { get; set; }     
    }
}