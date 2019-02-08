using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.Worker.TermsAndConditions.Models
{
    public class ConsentRequest
    {
        [Required]
        public bool ConsentGiven { get; set; }
        
        [Required]
        public bool AnalyticsCookieAccepted { get; set; }
        
        [Required]
        public bool UpdatingConsent { get; set; }
    }
}