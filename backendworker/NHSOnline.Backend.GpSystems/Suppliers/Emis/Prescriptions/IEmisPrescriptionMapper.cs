using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions
{
    public interface IEmisPrescriptionMapper
    {
        PrescriptionListResponse Map(PrescriptionRequestsGetResponse prescriptionGetResponse);

        CourseListResponse Map(CoursesGetResponse courseGetResponse);
    }
}
