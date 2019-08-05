using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.PfsApi.Areas.Session.Models
{
    [Serializable]
    public class UserSessionRequest
    {
        public string AuthCode { get; set; }
        
        public string CodeVerifier { get; set; }
        
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Uris are not serializable")]
        public string RedirectUrl { get; set; }
    }
}