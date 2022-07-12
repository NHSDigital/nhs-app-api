using System.Threading.Tasks;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public interface ISecondaryCareService
    {
        Task<SecondaryCareSummaryResult> GetSummary(P9UserSession userSession);
    }
}