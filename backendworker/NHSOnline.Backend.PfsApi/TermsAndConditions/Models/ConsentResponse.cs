using System;
using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions.Models
{
    public class ConsentResponse
    {
        [Required]
        public bool ConsentGiven { get; set; }

        [Required]
        public bool AnalyticsCookieAccepted { get; set; }

        [Required]
        public bool UpdatedConsentRequired { get; set; }

        [Required]
        public DateTimeOffset DateOfConsent { get; set; }
    }
}