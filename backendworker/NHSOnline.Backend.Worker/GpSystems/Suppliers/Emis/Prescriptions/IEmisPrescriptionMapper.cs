using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions
{
    public interface IEmisPrescriptionMapper
    {
        PrescriptionListResponse Map(PrescriptionRequestsGetResponse prescriptionGetResponse);

        CourseListResponse Map(CoursesGetResponse courseGetResponse);
    }
}
