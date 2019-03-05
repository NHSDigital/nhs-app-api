using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public interface IGpSearchService
    {
        Task<GpSearchResult> Search(string searchTerm);
    }
}