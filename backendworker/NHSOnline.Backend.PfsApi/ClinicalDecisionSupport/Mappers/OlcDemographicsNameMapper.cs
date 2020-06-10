using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Mappers
{
    internal class OlcDemographicsNameMapper : IMapper<string, DemographicsName, Models.Name>
    {
        private readonly IDictionary<string,string> _titlesMap;
        private const char SpaceChar = ' ';

        public OlcDemographicsNameMapper(IOlcDataMaps dataMaps)
        {
            _titlesMap = dataMaps.TitleDataMap;
        }

        public Models.Name Map(string firstSource, DemographicsName secondSource)
        {
            if (string.IsNullOrWhiteSpace(firstSource) && secondSource == null)
            {
                throw new ArgumentException($"{nameof(firstSource)} and {nameof(secondSource)} are both null/empty");
            }

            return Map(secondSource) ?? Map(firstSource);
        }

        private Models.Name Map(DemographicsName name)
        {
            return name != null
                ? new Models.Name
                {
                    GivenName = name.Given,
                    Surname = name.Surname,
                    Title = name.Title
                }
                : null;
        }

        private Models.Name Map(string fullName)
        {
            string surname, firstName = null;

            var title = _titlesMap.Where(t =>
                    fullName.StartsWith($"{t.Key}{SpaceChar}", StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Key)
                .FirstOrDefault();

            if (title != null)
            {
                fullName = fullName.Substring(title.Length + 1);
            }

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

            return new Models.Name
            {
                GivenName = firstName,
                Surname = surname,
                Title = MapPrefix(title)
            };
        }
        
        private string MapPrefix(string title)
        {
            return !string.IsNullOrWhiteSpace(title)
                   && _titlesMap.TryGetValue(title.ToUpperInvariant(), out var mappedTitle)
                ? mappedTitle
                : null;
        }
    }
}
