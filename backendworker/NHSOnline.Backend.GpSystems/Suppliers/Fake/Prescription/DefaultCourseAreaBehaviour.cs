using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    [FakeGpAreaBehaviour(Behaviour.Default)]
    public class DefaultCourseAreaBehaviour : ICourseAreaBehaviour
    {
        public async Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return await Task.FromResult<GetCoursesResult>(new GetCoursesResult.Forbidden());
        }
    }
}