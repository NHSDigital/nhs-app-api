using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models
{
    [Serializable]
    public class CreateJwtRequest
    {
        [Required]
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Uris are not serializable")]
        public string IntendedRelyingPartyUrl { get; set; }

        public string ProviderId { get; set; }

        public string ProviderName { get; set; }

        public string JumpOffId { get; set; }

        public string Action { get; set; }
    }
}