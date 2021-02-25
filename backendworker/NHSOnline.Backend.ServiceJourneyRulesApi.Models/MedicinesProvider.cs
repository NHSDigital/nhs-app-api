// ReSharper disable InconsistentNaming

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    /// <summary>
    /// The lowercase naming is required for serialization, as it currently
    /// does not support conversion from `PascalCase` to `camelCase`.
    /// </summary>
    public enum MedicinesProvider
    {
        pkb,
        pkbCie,
        pkbSecondaryCare,
        pkbMyCareView
    }
}