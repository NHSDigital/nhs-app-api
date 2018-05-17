using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Router.Prescriptions
{
    public interface ICourseService
    {
        Task<GetCoursesResult> Get(UserSession userSession);
    }
}
