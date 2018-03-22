using System;
using System.ComponentModel.DataAnnotations;
using NHSOnline.Backend.Worker.Ods;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection.Models
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
        [RegularExpression(OdsCodeFormats.GpPracticeEnglandWales)]
        public string OdsCode { get; set; }

        [Required]
        public string Surname { get; set; }
    }
}