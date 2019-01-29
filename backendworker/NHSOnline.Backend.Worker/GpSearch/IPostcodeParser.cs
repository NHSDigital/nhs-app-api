using NHSOnline.Backend.Worker.GpSearch.Models;

namespace NHSOnline.Backend.Worker.GpSearch
{
    public interface IPostcodeParser
    {
        ParsedPostcode ParseSearchTermForPostcodeMatch(string searchTerm);
    }
}