using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    [FakeGpArea("Course")]
    public interface ICourseAreaBehaviour
    {
        Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel);
    }
}