using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Models
{
    public class OrganDonationRegistrationRequest
    {
        [Required]
        public OrganDonationStoreRegistration Registration { get; set; }
        
        [Required]
        public AdditionalDetails AdditionalDetails { get; set; }
    }
}