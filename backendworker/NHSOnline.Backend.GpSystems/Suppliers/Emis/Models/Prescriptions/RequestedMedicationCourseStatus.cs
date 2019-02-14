using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions
{
    [SuppressMessage("Microsoft.Naming", "CA1717", Justification = "False Positive - 'Status' is not a plural")]
    public enum RequestedMedicationCourseStatus
    {
        Issued,
        Requested,
        ForwardedForSigning,
        Rejected,
        Unknown,
        Cancelled,
    }
}
