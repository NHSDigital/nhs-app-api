using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics
{
    public class FakeDemographicsService : FakeServiceBase, IDemographicsService
    {
        private readonly ILogger<FakeDemographicsService> _logger;

        public FakeDemographicsService(
            ILogger<FakeDemographicsService> logger,
            IFakeUserRepository fakeUserRepository)
        : base(logger, fakeUserRepository)
        {
            _logger = logger;
        }

        public async Task<DemographicsResult> GetDemographics(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            try
            {
                var fakeUser = await FindUser(gpLinkedAccountModel);
                return await fakeUser.DemographicsAreaBehaviour.GetDemographics(gpLinkedAccountModel, fakeUser);

            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}