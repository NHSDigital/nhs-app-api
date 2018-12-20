using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationReferenceDataResponseMapper :
        IMapper<OrganDonationResponse<ReferenceDataResponse>, OrganDonationReferenceDataResponse>
    {
        public OrganDonationReferenceDataResponse Map(OrganDonationResponse<ReferenceDataResponse> source)
        {
            if (source?.Body?.Entry == null)
                throw new ArgumentNullException(nameof(source));

            var entries = source.Body.Entry.ToDictionary(k => k.Resource.Id, v => v.Resource);

            return new OrganDonationReferenceDataResponse
            {
                Titles = MapOptions("titles", entries),
                Religions = MapOptions("religions", entries),
                Ethnicities = MapOptions("ethnicities", entries),
                Genders = MapOptions("genders", entries),
                WithdrawReasons = MapOptions("withdraw-reasons", entries)
            };
        }

        private List<SelectOption> MapOptions(string id, Dictionary<string, ReferenceDataResponse> entries)
        {
            return entries?.ContainsKey(id) == true
                ? entries[id].Concept
                    .Select(c => new SelectOption { DisplayName = c.Display, Id = c.Code })
                    .ToList()
                : new List<SelectOption>();
        }
    }
}