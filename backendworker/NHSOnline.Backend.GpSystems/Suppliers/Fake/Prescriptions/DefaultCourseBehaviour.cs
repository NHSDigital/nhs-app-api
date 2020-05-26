using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    public class DefaultCourseBehaviour : ICourseBehaviour
    {
        public async Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return await Task.FromResult<GetCoursesResult>(new GetCoursesResult.Forbidden());
        }
    }
}