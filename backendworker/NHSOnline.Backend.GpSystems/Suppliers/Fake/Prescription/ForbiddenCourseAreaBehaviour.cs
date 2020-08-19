using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    [FakeGpAreaBehaviour(Behaviour.Forbidden)]
    public class ForbiddenCourseAreaBehaviour : ICourseAreaBehaviour
    {
        public Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel)
        =>  Task.FromResult<GetCoursesResult>(new GetCoursesResult.Forbidden());
    }
}