using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationGenderMapper : IOrganDonationGenderMapper
    {
        private const string Unknown = "unknown";
        private const string Other = "other";

        private readonly Dictionary<string, string> _genderMap = new Dictionary<string, string>
        {
            { "MALE", "male" },
            { "FEMALE", "female" },
            { "TRANSGENDER", Other },
            { "NOTKNOWN", Other },
            { "INDETERMINATE", Other },
            { "NOTSPECIFIED", Other },
            { "UNSPECIFIED", Other }
        };

        public string Map(string source)
        {
            return !string.IsNullOrWhiteSpace(source) &&
                   _genderMap.TryGetValue(source.ToUpperInvariant(), out var genderCode)
                ? genderCode
                : Unknown;
        }
    }
}