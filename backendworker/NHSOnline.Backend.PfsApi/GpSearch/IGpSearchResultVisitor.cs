using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public interface IGpSearchResultVisitor<out T>
    {
        T Visit(GpSearchResult.Success result);
        T Visit(GpSearchResult.InternalServerError result);
        T Visit(GpSearchResult.BadGateway result);
        T Visit(GpSearchResult.BadRequest result);
    }
}