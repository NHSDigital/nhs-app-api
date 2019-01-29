using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSearch.Models;

namespace NHSOnline.Backend.Worker.GpSearch
{
    public interface IGpSearchService
    {
        Task<GpSearchResult> Search(string searchTerm);
    }
}