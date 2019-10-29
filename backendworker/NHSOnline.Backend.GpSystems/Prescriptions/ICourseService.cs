using NHSOnline.Backend.Support;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface ICourseService
    {
        Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel);
    }
}
