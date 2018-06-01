using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Router.Demographics
{
    public interface IDemographicsService
    {
        Task<GetMyRecordResult> Get(UserSession userSession);
    }
}