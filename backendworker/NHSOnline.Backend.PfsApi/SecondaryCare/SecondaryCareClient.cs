using System.Net;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareClient : ISecondaryCareClient
    {
        public SecondaryCareResponse<SummaryResponse> GetSummary()
        {
            var response = new SecondaryCareResponse<SummaryResponse>(HttpStatusCode.OK);
            return response.Parse();
        }
    }
}