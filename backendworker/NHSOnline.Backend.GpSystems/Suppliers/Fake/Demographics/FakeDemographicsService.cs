using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
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
                var fakeUser = FindUser(gpLinkedAccountModel);
                return await fakeUser.DemographicsBehaviour.GetDemographics(gpLinkedAccountModel, fakeUser);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return await Task.FromResult<DemographicsResult>(new DemographicsResult.InternalServerError());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}