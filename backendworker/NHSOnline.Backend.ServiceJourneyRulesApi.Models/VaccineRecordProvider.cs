using System.Diagnostics.CodeAnalysis;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models.Attributes;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    /// <summary>
    /// The lowercase naming is required for serialization, as it currently
    /// does not support conversion from `PascalCase` to `camelCase`.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum VaccineRecordProvider
    {
        nhsd,
        netCompany,

        [RemovesVaccineProvider(nhsd)]
        removeNhsd,
        [RemovesVaccineProvider(netCompany)]
        removeNetCompany
    }
}