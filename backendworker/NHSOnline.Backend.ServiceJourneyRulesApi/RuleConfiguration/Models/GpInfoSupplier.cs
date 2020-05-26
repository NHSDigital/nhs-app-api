using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    [SuppressMessage("Microsoft.Design", "CA1027", Justification = "This is not a flags enum")]
    public enum GpInfoSupplier
    {
        Unknown = Supplier.Unknown,

        [Description("EGTON MEDICAL INFORMATION SYSTEMS LTD (EMIS)")]
        Emis = Supplier.Emis,

        [Description("THE PHOENIX PARTNERSHIP")]
        Tpp = Supplier.Tpp,

        [Description("IN PRACTICE SYSTEMS LTD")]
        Vision = Supplier.Vision,

        [Description("MICROTEST LTD")]
        Microtest = Supplier.Microtest,

        [Description("FAKE SUPPLIER")]
        Fake = Supplier.Fake
    }
}