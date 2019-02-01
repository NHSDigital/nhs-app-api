using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.Worker.GpSystems.Prescriptions.Models
{
    [SuppressMessage("Microsoft.Naming", "CA1717", Justification = "False Positive - 'Status' is not a plural")]
    public enum Status
    {
        Unknown = 0,
        Rejected = 1,
        Requested = 2,
        Approved = 3,
    }
}
