using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    public interface ICourseBehaviour
    {
        Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel);
    }
}