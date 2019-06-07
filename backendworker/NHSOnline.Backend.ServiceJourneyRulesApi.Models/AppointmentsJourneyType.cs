// ReSharper disable InconsistentNaming

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    /// <summary>
    /// The valid journey types for appointments.
    ///
    /// The lowercase naming is required for serialization, as it currently
    /// does not support conversion from `PascalCase` to `camelCase`.
    /// </summary>
    public enum AppointmentsJourneyType
    {
        None = 0,
        disabled,
        im1Appointments
    }
}