using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions
{
    public static class RequestedMedicationCourseStatusExtensions
    {
        public static Status ToStatus(this RequestedMedicationCourseStatus value)
        {
            switch (value)
            {
                case RequestedMedicationCourseStatus.Issued:
                    return Status.Approved;
                case RequestedMedicationCourseStatus.Requested:
                case RequestedMedicationCourseStatus.ForwardedForSigning:
                    return Status.Requested;
                case RequestedMedicationCourseStatus.Rejected:
                    return Status.Rejected;
                default:
                    return Status.Unknown;
            }
        }
    }
}
