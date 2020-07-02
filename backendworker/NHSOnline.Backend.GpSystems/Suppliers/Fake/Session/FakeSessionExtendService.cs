using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    public class FakeSessionExtendService : FakeServiceBase, ISessionExtendService
    {
        private readonly ILogger<FakeSessionExtendService> _logger;

        public FakeSessionExtendService(
            ILogger<FakeSessionExtendService> logger,
            IFakeUserRepository fakeUserRepository)
            : base(logger, fakeUserRepository)
        {
            _logger = logger;
        }

        public async Task<SessionExtendResult> Extend(GpLinkedAccountModel gpLinkedAccountModel)
        {
            try
            {
                var fakeUser = await FindUser(gpLinkedAccountModel);
                return await fakeUser.SessionExtendAreaBehaviour.Extend(gpLinkedAccountModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return await Task.FromResult<SessionExtendResult>(new SessionExtendResult.InternalServerError());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}