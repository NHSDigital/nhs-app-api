using System;
using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.Worker.Areas.TermsAndConditions.Models
{
    public class ConsentRequest
    {
        [Required]
        public bool ConsentGiven { get; set; }
        [Required]
        public bool AnalyticsCookieAccepted { get; set; }        
    }
}