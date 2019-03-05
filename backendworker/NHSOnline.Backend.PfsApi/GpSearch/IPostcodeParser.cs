using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public interface IPostcodeParser
    {
        ParsedPostcode ParseSearchTermForPostcodeMatch(string searchTerm);
    }
}