using System.Text.RegularExpressions;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public class PostcodeParser: IPostcodeParser
    {
        public ParsedPostcode ParseSearchTermForPostcodeMatch(string searchTerm)
        {
            var matchedPostCode =
                new Regex(
                        "^((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]))))( ?[0-9][A-Za-z]{2})?)$")
                    .Match(searchTerm);

            var parsedPostcode = new ParsedPostcode();
            
            if (matchedPostCode.Success)
            {
                parsedPostcode.IsPostcode = true;
                parsedPostcode.Postcode = matchedPostCode.Groups[0].Value;
                parsedPostcode.Inward = matchedPostCode.Groups[9].Value;
            }
            
            return parsedPostcode;
        }
    }
}