using NHSOnline.Backend.Worker.GpSearch.Models;

namespace NHSOnline.Backend.Worker.GpSearch.GpLookup
{
    public interface IGpSearchResultVisitor<out T>
    {
        T Visit(GpSearchResult.SuccessfullyRetrieved result);
        T Visit(GpSearchResult.Unsuccessful result);
        T Visit(GpSearchResult.NhsSearchServiceUnavailable result);
        T Visit(GpSearchResult.BadRequest result);
        
    }
}