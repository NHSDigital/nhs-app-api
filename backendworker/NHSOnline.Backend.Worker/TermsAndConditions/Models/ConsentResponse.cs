using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.Worker.TermsAndConditions.Models
{
    public class ConsentResponse
    {
        [Required]
        public bool ConsentGiven { get; set; }
        
        [Required]
        public bool AnalyticsCookieAccepted { get; set; }
        
        [Required]
        public bool UpdatedConsentRequired { get; set; } 
    }
}