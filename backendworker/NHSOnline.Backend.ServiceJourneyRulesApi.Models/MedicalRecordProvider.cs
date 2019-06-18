// ReSharper disable InconsistentNaming

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    /// <summary>
    /// The valid providers for medical record.
    ///
    /// The lowercase naming is required for serialization, as it currently
    /// does not support conversion from `PascalCase` to `camelCase`.
    /// </summary>
    public enum MedicalRecordProvider
    {
        Unknown = 0,
        none,
        im1
    }
}