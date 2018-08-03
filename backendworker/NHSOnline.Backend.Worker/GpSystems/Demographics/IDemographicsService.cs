using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Demographics
{
    public interface IDemographicsService
    {
        Task<GetDemographicsResult> GetDemographics(UserSession userSession);
    }
}