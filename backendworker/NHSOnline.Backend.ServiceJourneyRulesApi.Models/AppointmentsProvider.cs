// ReSharper disable InconsistentNaming

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    /// <summary>
    /// The valid providers for appointments.
    ///
    /// The lowercase naming is required for serialization, as it currently
    /// does not support conversion from `PascalCase` to `camelCase`.
    /// </summary>
    public enum AppointmentsProvider
    {
        im1,
        informatica,
        gpAtHand,
        linkedAccount,
        eConsult
    }
}