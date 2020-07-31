using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Linkage
{
    public class FakeLinkageService : FakeServiceBase, ILinkageService
    {
        private readonly ILogger<FakeLinkageService> _logger;

        public FakeLinkageService(
            ILogger<FakeLinkageService> logger,
            IFakeUserRepository fakeUserRepository)
        : base(logger, fakeUserRepository)
        {
            _logger = logger;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            _logger.LogEnter();

            try
            {
                var fakeUser = await FindUser(getLinkageRequest.NhsNumber);
                return await fakeUser.LinkageAreaBehaviour.GetLinkageKey(getLinkageRequest);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            return await Task.FromResult(new LinkageResult.NotFound(Im1ConnectionErrorCodes.InternalCode.UnknownError));
        }
    }
}