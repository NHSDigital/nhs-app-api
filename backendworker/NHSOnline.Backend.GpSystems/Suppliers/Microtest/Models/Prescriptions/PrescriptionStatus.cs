using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions
{
    [SuppressMessage("Microsoft.Naming", "CA1717", Justification = "False Positive - 'Status' is not a plural")]
    public static class PrescriptionStatus
    {
        public const string Requested = "Requested";
        public const string Rejected = "Rejected";
        public const string Cancelled = "Cancelled";
        public const string Confirmed = "Confirmed";
    }
}
