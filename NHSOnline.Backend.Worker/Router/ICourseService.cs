using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Router.Prescriptions;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Router
{
    public interface ICourseService
    {
        Task<GetCoursesResult> Get(UserSession userSession);
    }
}
