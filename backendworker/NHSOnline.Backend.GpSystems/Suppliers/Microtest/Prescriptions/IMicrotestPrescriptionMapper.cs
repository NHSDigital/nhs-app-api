using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Prescriptions
{
    public interface IMicrotestPrescriptionMapper
    {
        PrescriptionListResponse Map(PrescriptionHistoryGetResponse prescriptionHistoryGetResponse);

        CourseListResponse Map(CoursesGetResponse courseGetResponse);
    }
}
