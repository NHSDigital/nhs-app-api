using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationAddressMapper : IMapper<string, OrganDonation.Models.Address, Address>
    {
        private static readonly Regex PostCodeRegex =
            new Regex(@"([A-Za-z][A-Ha-hJ-Yj-y]?[0-9][A-Za-z0-9]? ?[0-9][A-Za-z]{2}|[Gg][Ii][Rr] ?0[Aa]{2})");

        public Address Map(string firstSource, OrganDonation.Models.Address secondSource)
        {
            if (string.IsNullOrWhiteSpace(firstSource) && secondSource == null)
            {
                throw new ArgumentException($"{nameof(firstSource)} and {nameof(secondSource)} are both null/empty");
            }

            return Map(secondSource) ?? Map(firstSource);
        }

        private Address Map(string fullAddress)
        {
            var postCodeMatches = PostCodeRegex.Matches(fullAddress);
            string postCode, line;

            if (postCodeMatches.Any())
            {
                postCode = postCodeMatches.Last().Value;
                line = fullAddress.Replace(postCode, string.Empty, StringComparison.Ordinal)
                    .TrimEnd();
            }
            else
            {
                postCode = null;
                line = fullAddress;
            }

            return new Address
            {
                Line = new List<string> { line },
                PostalCode = postCode
            };
        }

        private Address Map(OrganDonation.Models.Address address) => address != null
            ? new Address
            {
                PostalCode = address.PostCode,
                Line = new List<string> { address.Text }
            }
            : null;
    }
}