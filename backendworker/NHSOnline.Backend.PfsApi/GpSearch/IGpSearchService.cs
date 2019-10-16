using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public interface IGpSearchService
    {
        Task<GpSearchResult> Search(string searchTerm);

        Task<GpSearchResult> GetGpPracticeByOdsCode(string odsCode);

        Task<IsGpPracticeEpsEnabledResponse> IsGpPracticeEPSEnabled(string odsCode);
    }
}
