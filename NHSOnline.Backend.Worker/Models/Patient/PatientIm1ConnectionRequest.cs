using System;

namespace NHSOnline.Backend.Worker.Models.Patient
{
    public class PatientIm1ConnectionRequest
    {
        public string AccountId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string LinkageKey { get; set; }
        public string OdsCode { get; set; }
        public string Surname { get; set; }
    }
}
