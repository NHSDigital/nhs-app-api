using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.Worker.Areas.Session.Models
{
    [Serializable]
    public class UserSessionRequest
    {
        [Required]
        public string AuthCode { get; set; }

        [Required]
        public string CodeVerifier { get; set; }

        [Required]
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Uris are not serializable")]
        public string RedirectUrl { get; set; }
    }
}