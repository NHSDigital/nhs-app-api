using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Prescriptions
{
    public interface IVisionPrescriptionMapper
    {
        PrescriptionListResponse Map(PrescriptionHistory prescriptionGetResponse);

        CourseListResponse Map(EligibleRepeats eligibleRepeatsResponse);
    }
}
