using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.Worker.Areas.Session.Models
{
    public class UserSessionRequest
    {
        [Required]
        public string AuthCode { get; set; }

        [Required]
        public string CodeVerifier { get; set; }

        [Required]
        public string RedirectUrl { get; set; }
    }
}