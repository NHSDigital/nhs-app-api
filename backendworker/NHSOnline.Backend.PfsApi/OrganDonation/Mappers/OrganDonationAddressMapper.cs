using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class OrganDonationAddressMapper : IMapper<string, Address, ApiModels.Address>
    {
        private const char Delimiter = ',';
        private static readonly Regex PostCodeRegex =
            new Regex(@"([A-Za-z][A-Ha-hJ-Yj-y]?[0-9][A-Za-z0-9]? ?[0-9][A-Za-z]{2}|[Gg][Ii][Rr] ?0[Aa]{2})");

        private readonly ILogger<OrganDonationAddressMapper> _logger;

        public OrganDonationAddressMapper(ILogger<OrganDonationAddressMapper> logger)
        {
            _logger = logger;
        }

        public ApiModels.Address Map(string firstSource, Address secondSource)
        {
            if (string.IsNullOrWhiteSpace(firstSource) && secondSource == null)
            {
                throw new ArgumentException($"{nameof(firstSource)} and {nameof(secondSource)} are both null/empty");
            }

            return Map(secondSource) ?? Map(firstSource);
        }

        private ApiModels.Address Map(Address address)
        {
            if (address == null)
            {
                return null;
            }

            var parts = new List<string>
                {
                    address.HouseName,
                    address.NumberStreet,
                    address.Village,
                }
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            return MapPartsToAddress(parts, address.PostCode, address.Town, address.County);
        }

        private ApiModels.Address Map(string fullAddress)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(fullAddress, nameof(fullAddress), ValidateAndLog.ValidationOptions.ThrowError)
                .IsValid();

            var postCodeMatches = PostCodeRegex.Matches(fullAddress);

            new ValidateAndLog(_logger)
                .IsListPopulated(postCodeMatches, nameof(fullAddress), ValidateAndLog.ValidationOptions.ThrowError)
                .IsValid();
            
            var postCode = postCodeMatches.Last().Value;
            var line = fullAddress.Replace(postCode, string.Empty, StringComparison.Ordinal);
            return SplitAddressLineAndMap(line, postCode);
        }

        private ApiModels.Address SplitAddressLineAndMap(string line, string postCode)
        {
            var parts = line.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim())
                .ToList();

            switch (parts.Count)
            {
                case var n when n >= 4:
                    return MapPartsToAddress(parts.Take(2).ToList(), postCode, parts[2], JoinString(parts.Skip(3)));
                case 3:
                    return MapPartsToAddress(parts.Take(2).ToList(), postCode, parts.Last());
                default:
                    return MapPartsToAddress(parts, postCode);
            }
        }

        private ApiModels.Address MapPartsToAddress(List<string> parts, string postCode, string city = null, string district = null)
        {
            return new ApiModels.Address
            {
                Line = parts.Count > 2
                    ? new List<string>
                    {
                        parts.First(),
                        JoinString(parts.Skip(1)),
                    }
                    : parts,
                City = city,
                District = district,
                PostalCode = postCode
            };
        }

        private static string JoinString(IEnumerable<string> parts) => string.Join($"{Delimiter} ", parts);
    }
}