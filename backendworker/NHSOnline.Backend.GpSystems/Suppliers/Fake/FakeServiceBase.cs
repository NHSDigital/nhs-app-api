using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public abstract class FakeServiceBase
    {
        private readonly ILogger _logger;
        private readonly IFakeUserRepository _fakeUserRepository;

        protected FakeServiceBase(
            ILogger logger,
            IFakeUserRepository fakeUserRepository)
        {
            _logger = logger;
            _fakeUserRepository = fakeUserRepository;
        }

        protected IFakeUser FindUser(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var session = (FakeUserSession) gpLinkedAccountModel.GpUserSession;
            return FindUser(session.NhsNumber);
        }

        protected IFakeUser FindUser(string nhsNumber)
        {
            var user = _fakeUserRepository.Find(nhsNumber);
            return user;
        }
    }
}