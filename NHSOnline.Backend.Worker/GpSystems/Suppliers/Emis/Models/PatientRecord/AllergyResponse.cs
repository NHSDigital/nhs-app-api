using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class AllergyResponse
    {
        public string Term { get; set; }
        public DateTimeOffset AvailabilityDateTime { get; set; }     
    }
}