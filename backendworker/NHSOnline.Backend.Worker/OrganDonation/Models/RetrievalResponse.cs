using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    internal abstract class RetrievalResponse<TBody>
    {
        public List<Entry<TBody>> Entry { get; set; } 
    }
}