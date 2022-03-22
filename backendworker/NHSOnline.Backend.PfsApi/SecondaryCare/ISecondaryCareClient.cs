using NHSOnline.Backend.PfsApi.SecondaryCare.Models;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public interface ISecondaryCareClient
    {
        SecondaryCareResponse<SummaryResponse> GetSummary();
    }
}