// ReSharper disable InconsistentNaming

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    /// <summary>
    /// The valid journey types for medical record.
    ///
    /// The lowercase naming is required for serialization, as it currently
    /// does not support conversion from `PascalCase` to `camelCase`.
    /// </summary>
    public enum MedicalRecordJourneyType
    {
        None = 0,
        disabled,
        im1MedicalRecord
    }
}