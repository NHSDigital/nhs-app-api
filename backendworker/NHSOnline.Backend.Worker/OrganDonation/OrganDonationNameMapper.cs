using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationNameMapper : IMapper<string, Areas.OrganDonation.Models.Name, Name>
    {
        private const char SpaceChar = ' ';

        private readonly Dictionary<string, string> _titlesMap = new Dictionary<string, string>
        {
            { "MR", "MR" },
            { "MRS", "MRS" },
            { "MISS", "MISS" },
            { "MS", "MS" },
            { "MX", "MX" },
            { "MASTER", "MASTER" },
            { "DOCTOR", "DR" },
            { "DR", "DR" },
            { "CLLR", "CLLR" },
            { "COUNCILLOR", "CLLR" },
            { "CAPT", "CAPT" },
            { "CAPTAIN", "CAPT" },
            { "COLONEL", "COLONEL" },
            { "EXORS", "EXORS" },
            { "EXECUTORS OF", "EXORS" },
            { "FR", "FR" },
            { "FATHER", "FR" },
            { "LADY", "LADY" },
            { "LORD", "LORD" },
            { "PROFESSOR", "PROF" },
            { "REV", "REV" },
            { "REVEREND", "REV" },
            { "SIR", "SIR" },
            { "DAME", "DAME" }
        };

        public Name Map(string firstSource, Areas.OrganDonation.Models.Name secondSource)
        {
            if (string.IsNullOrWhiteSpace(firstSource) && secondSource == null)
            {
                throw new ArgumentException($"{nameof(firstSource)} and {nameof(secondSource)} are both null/empty");
            }

            return Map(secondSource) ?? Map(firstSource);
        }

        private Name Map(Areas.OrganDonation.Models.Name name)
        {
            return name != null
                ? new Name
                {
                    Given = MapGiven(name.GivenName),
                    Family = name.Surname,
                    Prefix = MapPrefix(name.Title)
                }
                : null;
        }

        private Name Map(string fullName)
        {
            string surname, firstName = null;
            
            var title = _titlesMap.Where(t =>
                    fullName.StartsWith($"{t.Key}{SpaceChar}", StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Key)
                .FirstOrDefault();

            if (title != null)
                fullName = fullName.Substring(title.Length + 1);

            var nameParts = fullName.Split(SpaceChar, StringSplitOptions.RemoveEmptyEntries);

            switch (nameParts.Length)
            {
                case 0:
                    surname = title;
                    title = null;
                    break;
                case var length when length == 1 && title != null:
                    firstName = title;
                    surname = nameParts.First();
                    title = null;
                    break;
                case var length when length >= 2:
                    surname = nameParts.Last();
                    firstName = string.Join(SpaceChar, nameParts.SkipLast(1));
                    break;
                default:
                    surname = nameParts.First();
                    break;
            }

            return new Name
            {
                Given = MapGiven(firstName),
                Family = surname,
                Prefix = MapPrefix(title)
            };
        }

        private List<string> MapPrefix(string title)
        {
            return !string.IsNullOrWhiteSpace(title)
                   && _titlesMap.TryGetValue(title.ToUpperInvariant(), out var mappedTitle)
                ? new List<string> { mappedTitle }
                : null;
        }

        private static List<string> MapGiven(string firstName) =>
            firstName != null ? new List<string> { firstName } : null;
    }
}