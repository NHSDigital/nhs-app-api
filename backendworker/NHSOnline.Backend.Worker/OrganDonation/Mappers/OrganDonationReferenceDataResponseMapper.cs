using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation.Mappers
{
    internal class OrganDonationReferenceDataResponseMapper :
        IMapper<OrganDonationResponse<ReferenceDataResponse>, OrganDonationReferenceDataResponse>
    {
        public OrganDonationReferenceDataResponse Map(OrganDonationResponse<ReferenceDataResponse> source)
        {
            if (source?.Body?.Entry == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

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

        private List<SelectOption> MapOptions(string id, Dictionary<string, ReferenceData> entries)
        {
            return entries?.ContainsKey(id) == true
                ? entries[id].Concept
                    .Select(c => new SelectOption { DisplayName = c.Display, Id = c.Code })
                    .ToList()
                : new List<SelectOption>();
        }
    }
}