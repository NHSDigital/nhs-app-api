using System;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models.PatientRecord
{
    public class AllergyResponse
    {
        public string AllergyName { get; set; }
        public DateTimeOffset AvailabilityDateTime { get; set; }     
    }
}