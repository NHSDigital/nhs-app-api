using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Support;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;

namespace NHSOnline.Backend.Worker.OrganDonation.Mappers
{
    internal class OrganDonationNameMapper : IMapper<OrganDonation.Models.Name, Name>
    {
        private readonly ILogger<OrganDonationNameMapper> _logger;
        private readonly IDictionary<string,string> _titlesMap;
        private const char SpaceChar = ' ';

        public OrganDonationNameMapper(IOrganDonationDataMaps dataMaps, ILogger<OrganDonationNameMapper> logger)
        {
            _logger = logger;
            _titlesMap = dataMaps.TitleDataMap;
        }

        public Name Map(OrganDonation.Models.Name source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ValidateAndLog.ValidationOptions.ThrowError)
                .IsValid();

            var prefix = MapPrefix(source.Title);

            return new Name
                {
                    Given = MapGiven(prefix, source),
                    Family = source.Surname,
                    Prefix = prefix
                };
        }

        private static List<string> MapGiven(List<string> prefix, OrganDonation.Models.Name source)
        {
            var givenName = source.GivenName;
            if (prefix == null && !string.IsNullOrWhiteSpace(source.Title))
            {
                givenName = string.IsNullOrWhiteSpace(source.GivenName)
                    ? $"{source.Title}"
                    : $"{source.Title}{SpaceChar}{source.GivenName}";
            }

            return string.IsNullOrWhiteSpace(givenName) ? null : new List<string> { givenName };
        }

        private List<string> MapPrefix(string title)
        {
            return !string.IsNullOrWhiteSpace(title)
                   && _titlesMap.TryGetValue(title.ToUpperInvariant(), out var mappedTitle)
                ? new List<string> { mappedTitle }
                : null;
        }
    }
}