using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics
{
    [FakeGpAreaBehaviour(Behaviour.Unauthorised)]
    public class UnauthorisedDemographicsAreaBehaviour : IDemographicsAreaBehaviour
    {
        public Task<DemographicsResult> GetDemographics(GpLinkedAccountModel gpLinkedAccountModel,
            FakeUser fakeUser) => throw new UnauthorisedGpSystemHttpRequestException();
    }
}