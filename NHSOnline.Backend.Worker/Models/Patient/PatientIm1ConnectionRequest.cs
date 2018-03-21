using System;
using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.Worker.Models.Patient
{
    public class PatientIm1ConnectionRequest
    {
        [Required]
        public string AccountId { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string LinkageKey { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z0-9]{1,8}$")]
        public string OdsCode { get; set; }

        [Required]
        public string Surname { get; set; }
    }
}