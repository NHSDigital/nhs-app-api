using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.Worker.GpSearch.Models.Pharmacy;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public interface IGpSearchService
    {
        Task<GpSearchResult> Search(string searchTerm);

        Task<IsGpPracticeEpsEnabledResponse> IsGpPracticeEPSEnabled(string odsCode);
    }
}
