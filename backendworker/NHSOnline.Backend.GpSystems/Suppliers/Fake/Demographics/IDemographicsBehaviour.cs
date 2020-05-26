using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics
{
    public interface IDemographicsBehaviour
    {
        Task<DemographicsResult> GetDemographics(GpLinkedAccountModel gpLinkedAccountModel,
            IFakeUser fakeUser);
    }
}