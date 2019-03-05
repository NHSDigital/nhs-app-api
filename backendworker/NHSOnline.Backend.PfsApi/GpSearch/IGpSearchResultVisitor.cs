using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch.GpLookup
{
    public interface IGpSearchResultVisitor<out T>
    {
        T Visit(GpSearchResult.SuccessfullyRetrieved result);
        T Visit(GpSearchResult.Unsuccessful result);
        T Visit(GpSearchResult.NhsSearchServiceUnavailable result);
        T Visit(GpSearchResult.BadRequest result);
        
    }
}