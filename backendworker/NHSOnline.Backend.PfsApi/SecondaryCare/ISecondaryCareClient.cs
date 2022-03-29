using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public interface ISecondaryCareClient
    {
        Task<SecondaryCareResponse<SummaryResponse>> GetSummary(P9UserSession userSession);
    }
}