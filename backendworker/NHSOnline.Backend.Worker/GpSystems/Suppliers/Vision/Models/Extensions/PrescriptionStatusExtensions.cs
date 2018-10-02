using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Extensions
{
    public static class PrescriptionStatusExtensions
    {
        public static Areas.Prescriptions.Models.Status ToStatus(this Status value)
        {
            switch (value.Code)
            {
                case PrescriptionRepeatStatusCode.Processed:
                    return Areas.Prescriptions.Models.Status.Approved;

                case PrescriptionRepeatStatusCode.Rejected:
                    return Areas.Prescriptions.Models.Status.Rejected;

                case PrescriptionRepeatStatusCode.InProgress:
                    return Areas.Prescriptions.Models.Status.Requested;

                case PrescriptionRepeatStatusCode.NotProcessed:
                default:
                    return Areas.Prescriptions.Models.Status.Unknown;
            }
        }
    }
}
