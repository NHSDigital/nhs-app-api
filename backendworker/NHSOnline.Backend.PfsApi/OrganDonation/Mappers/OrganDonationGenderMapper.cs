using System.Collections.Generic;
using NHSOnline.Backend.PfsApi.OrganDonation;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class OrganDonationGenderMapper : IOrganDonationGenderMapper
    {
        private readonly IDictionary<string, string> _genderMap;
        private const string Unknown = "unknown";

        public OrganDonationGenderMapper(IOrganDonationDataMaps dataMaps)
        {
            _genderMap = dataMaps.GenderDataMap;
        }

        public string Map(string source)
        {
            return !string.IsNullOrWhiteSpace(source) &&
                   _genderMap.TryGetValue(source.ToUpperInvariant(), out var genderCode)
                ? genderCode
                : Unknown;
        }
    }
}