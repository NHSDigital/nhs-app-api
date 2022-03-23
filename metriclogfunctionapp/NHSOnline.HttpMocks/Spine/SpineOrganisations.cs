using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace NHSOnline.HttpMocks.Spine
{
    public sealed class SpineOrganisationsResponse
    {
        private readonly List<SpineOrganisation> _organisations = new List<SpineOrganisation>();

        public void Add(SpineOrganisation spineOrganisation)
        {
            _organisations.Add(spineOrganisation);
        }

        public void Reset()
        {
            _organisations.Clear();
        }

        internal bool TryGetByOdsCode(string odsCode, out SpineOrganisation spineOrganisation)
        {
            spineOrganisation = _organisations.FirstOrDefault(org => org.OrgId == odsCode);
            return spineOrganisation != null;
        }

        internal int TotalCount()
        {
            return _organisations.Count;
        }

        internal JObject SearchResults()
        {
            var organisations = _organisations.Select(org => org.SearchResult()).ToArray<object>();
            return MapToOrganisationsObject(organisations);
        }

        internal JObject SearchResultsExceedingLimit()
        {
            var organisations = _organisations.Select(org => org.SearchResult()).ToArray<object>();
            var count = Math.Min(organisations.Length - 1000, 1000);
            var offsetOrganisations = organisations.ToList().GetRange(1000, count);
            return MapToOrganisationsObject(offsetOrganisations.ToArray());
        }

        private JObject MapToOrganisationsObject(object[] organisations)
        {
            return new JObject(new JProperty("Organisations", new JArray(organisations)));
        }
    }
}