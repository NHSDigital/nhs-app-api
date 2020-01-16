using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Internal;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public static class OrganisationSearchUtility
    {
        public static string PrepareSearch(string searchTerm, bool matchWhole = false)
        {
            if (matchWhole)
            {
                return $"\"{searchTerm}\"";
            }

            var parts = searchTerm.Split(' ').Join("*+");
            return $"{parts}*";
        }

        public static PostcodeSearchData CreatePostcodeSearchQuery(string searchTerm, bool outcodeOnly)
        {
            return new PostcodeSearchData
            {
                Top = 1,
                Search = searchTerm,
                Count = true,
                Filter = GetPostcodesAndPlacesFilter(outcodeOnly),
            };
        }

        public static string GetPostcodesAndPlacesFilter(bool outcodeOnly)
        {
            return outcodeOnly ? "Type eq 'PostcodeOutCode'" : "LocalType eq 'Postcode'";
        }

        public static string SanitizeSearch(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return searchTerm;
            }

            var dashesRemoved = searchTerm.Trim().Replace('-', ' ');
            var multipleSpacesRemoved = Regex.Replace(dashesRemoved, "\\s\\s+/g", " ");
            var sanitisedSearchCriteria = Regex.Replace(multipleSpacesRemoved, "[/\\\\^$*+&?,.()|[\\]{}\"~:!<>£;@%^'`]/g", string.Empty);

            return sanitisedSearchCriteria;
        }
    }
}