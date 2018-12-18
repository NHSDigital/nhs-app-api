using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    public class OrganDonationSuccessResponse<TBody>
    {
        public List<Entry<TBody>> Entry { get; set; }
    }
}