using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    [FakeGpAreaBehaviour(Behaviour.Unauthorised)]
    public class UnauthorisedCourseAreaBehaviour: ICourseAreaBehaviour
    {
        public Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel)
        {
            throw new UnauthorisedGpSystemHttpRequestException();
        }
    }
}