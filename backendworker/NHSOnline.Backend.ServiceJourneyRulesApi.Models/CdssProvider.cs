// ReSharper disable InconsistentNaming

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    /// <summary>
    /// The valid providers for cdss.
    ///
    /// The lowercase naming is required for serialization, as it currently
    /// does not support conversion from `PascalCase` to `camelCase`.
    /// </summary>
    public enum CdssProvider
    {
        none,
        eConsult,
        stubs
    }
}