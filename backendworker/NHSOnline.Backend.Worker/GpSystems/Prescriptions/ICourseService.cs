using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Prescriptions
{
    public interface ICourseService
    {
        Task<GetCoursesResult> GetCourses(UserSession userSession);
    }
}
