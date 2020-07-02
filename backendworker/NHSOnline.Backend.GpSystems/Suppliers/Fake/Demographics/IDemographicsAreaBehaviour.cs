using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics
{
    [FakeGpArea("Demographics")]
    public interface IDemographicsAreaBehaviour
    {
        Task<DemographicsResult> GetDemographics(GpLinkedAccountModel gpLinkedAccountModel,
            FakeUser fakeUser);
    }
}