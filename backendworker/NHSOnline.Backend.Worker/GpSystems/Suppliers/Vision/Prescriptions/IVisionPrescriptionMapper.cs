using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Prescriptions
{
    public interface IVisionPrescriptionMapper
    {
        PrescriptionListResponse Map(PrescriptionHistory prescriptionGetResponse);
    }
}
