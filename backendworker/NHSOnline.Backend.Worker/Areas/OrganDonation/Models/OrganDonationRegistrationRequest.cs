namespace NHSOnline.Backend.Worker.Areas.OrganDonation.Models
{
    public class OrganDonationRegistrationRequest
    {
        public OrganDonationRegistration Registration { get; set; }
        
        public AdditionalDetails AdditionalDetails { get; set; }
    }
}