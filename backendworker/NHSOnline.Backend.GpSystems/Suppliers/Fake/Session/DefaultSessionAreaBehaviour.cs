using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    [FakeGpAreaBehaviour(Behaviour.Default)]
    public class DefaultSessionAreaBehaviour : ISessionAreaBehaviour
    {
        private readonly IFakeUserRepository _userRepository;

        public DefaultSessionAreaBehaviour(IFakeUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GpSessionCreateResult> Create(
            string connectionToken,
            string odsCode,
            string nhsNumber,
            FakeUser user
        )
        {
            var gpUserSession = new FakeUserSession
            {
                Id = user.UserUuid,
                NhsNumber = user.NhsNumber,
                OdsCode = user.OdsCode,
                Name = user.Name,
                ProxyPatients = new List<FakeProxyUserSession>()
            };

            foreach (var proxyNhsNumber in user.LinkedAccountsNhsNumbers)
            {
                var proxyUser = await _userRepository.Find(proxyNhsNumber);
                gpUserSession.ProxyPatients.Add(new FakeProxyUserSession
                {
                    Id = proxyUser.UserUuid,
                    NhsNumber = proxyUser.NhsNumber,
                    OdsCode = proxyUser.OdsCode
                });
            }

            return await Task.FromResult<GpSessionCreateResult>(new GpSessionCreateResult.Success(gpUserSession));
        }

        public async Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            return await Task.FromResult<SessionLogoffResult>(new SessionLogoffResult.Success(gpUserSession));
        }

        public Task<GpSessionRecreateResult> Recreate(string connectionToken, string odsCode, string nhsNumber,
            string patientId)
        {
            throw new System.NotImplementedException();
        }
    }
}