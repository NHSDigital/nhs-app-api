using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
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

        protected Task<FakeUser> FindUser(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var session = gpLinkedAccountModel.GpUserSession as FakeUserSession;

            return FindUser(session?.NhsNumber);
        }

        protected Task<FakeUser> FindUser(string nhsNumber)
        {
            return _fakeUserRepository.Find(nhsNumber);
        }
    }
}