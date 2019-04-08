using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class OrganDonationAddressMapper : IMapper<string, Models.Address, Address>
    {
        private const char Delimiter = ',';
        private static readonly Regex PostCodeRegex =
            new Regex(@"([A-Za-z][A-Ha-hJ-Yj-y]?[0-9][A-Za-z0-9]? ?[0-9][A-Za-z]{2}|[Gg][Ii][Rr] ?0[Aa]{2})");

        private readonly ILogger<OrganDonationAddressMapper> _logger;

        public OrganDonationAddressMapper(ILogger<OrganDonationAddressMapper> logger)
        {
            _logger = logger;
        }

        public Address Map(string firstSource, Models.Address secondSource)
        {
            if (string.IsNullOrWhiteSpace(firstSource) && secondSource == null)
            {
                throw new ArgumentException($"{nameof(firstSource)} and {nameof(secondSource)} are both null/empty");
            }

            return Map(secondSource) ?? Map(firstSource);
        }

        private Address Map(string fullAddress)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(fullAddress, nameof(fullAddress), ValidateAndLog.ValidationOptions.ThrowError)
                .IsValid();

            var postCodeMatches = PostCodeRegex.Matches(fullAddress);
            string postCode, line;

            if (postCodeMatches.Any())
            {
                postCode = postCodeMatches.Last().Value;
                line = fullAddress.Replace(postCode, string.Empty, StringComparison.Ordinal);
            }
            else
            {
                postCode = null;
                line = fullAddress;
            }

            var parts = line.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim())
                .ToList();
            
            return Map(postCode, parts);
        }

        private Address Map(Models.Address address)
        {
            if (address == null) return null;
            
            var parts = new List<string> {
                    address.HouseName,
                    address.NumberStreet,
                    address.Village,
                    address.Town,
                    address.County
                }
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            return Map(address.PostCode, parts);
        }

        private Address Map(string postCode, List<string> parts)
        {
            var mappedAddress = new Address
            {
                PostalCode = postCode
            };

            switch (parts.Count)
            {
                case int n when n > 4:
                    mappedAddress.Line = new List<string>(parts.Take(3));
                    mappedAddress.Line.Add(string.Join($"{Delimiter} ", parts.Skip(3)));
                    break;
                default:
                    mappedAddress.Line = parts;
                    break;
            }

            return mappedAddress;
        }
    }
}
