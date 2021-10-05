using System.ComponentModel;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    public enum GpInfoSupplier
    {
        Unknown,

        [Description("EGTON MEDICAL INFORMATION SYSTEMS LTD (EMIS)")]
        Emis,

        [Description("THE PHOENIX PARTNERSHIP")]
        Tpp,

        [Description("IN PRACTICE SYSTEMS LTD")]
        Vision,

        [Description("FAKE SUPPLIER")]
        Fake
    }
}