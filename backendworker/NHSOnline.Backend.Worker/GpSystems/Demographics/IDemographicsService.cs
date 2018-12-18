using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Demographics
{
    public interface IDemographicsService
    {
        Task<DemographicsResult> GetDemographics(UserSession userSession);
    }
}