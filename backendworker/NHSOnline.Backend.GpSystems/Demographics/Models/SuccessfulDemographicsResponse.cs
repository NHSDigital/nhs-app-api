using System;

namespace NHSOnline.Backend.GpSystems.Demographics.Models
{
    public class SuccessfulDemographicsResponse
    {
        public string PatientName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string Address { get; set; }
        public string NhsNumber { get; set; }
    }
}