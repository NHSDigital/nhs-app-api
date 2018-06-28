using System;

namespace NHSOnline.Backend.Worker.Areas.Demographics.Models
{
    public class DemographicsResponse
    {
        public string PatientName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string Address { get; set; }
        public string NhsNumber { get; set; }
    }
}