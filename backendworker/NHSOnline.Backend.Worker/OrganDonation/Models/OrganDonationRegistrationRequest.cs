using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    public class OrganDonationRegistrationRequest
    {
        [Required]
        public OrganDonationStoreRegistration Registration { get; set; }
        
        [Required]
        public AdditionalDetails AdditionalDetails { get; set; }
    }
}