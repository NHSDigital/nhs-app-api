namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    public class OrganDonationSuccessResponse<TBody>
    {
        public Entry<TBody> Entry { get; set; }
    }
}