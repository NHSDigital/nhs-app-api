using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Demographics
{
    public interface IEmisDemographicsService
    {
        Task<DemographicsResult> GetDemographics(GpUserSession gpUserSession);
    }
}
