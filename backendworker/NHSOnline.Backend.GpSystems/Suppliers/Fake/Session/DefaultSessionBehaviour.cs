using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    public class DefaultSessionBehaviour : ISessionBehaviour
    {
        private readonly IFakeUserRepository _userRepository;

        public DefaultSessionBehaviour(IFakeUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber,
            IFakeUser user)
        {
            var gpUserSession = new FakeUserSession
            {
                Id = user.Id,
                NhsNumber = user.NhsNumber,
                OdsCode = user.OdsCode,
                Name = user.Name,
                ProxyPatients = new List<FakeProxyUserSession>()
            };

            foreach (var proxyNhsNumber in user.LinkedAccountsNhsNumbers)
            {
                var proxyUser = _userRepository.Find(proxyNhsNumber);
                gpUserSession.ProxyPatients.Add(new FakeProxyUserSession
                {
                    Id = proxyUser.Id,
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