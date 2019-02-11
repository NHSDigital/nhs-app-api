using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation.ApiModels
{
    internal abstract class RetrievalResponse<TBody>
    {
        public List<Entry<TBody>> Entry { get; set; }
    }
}