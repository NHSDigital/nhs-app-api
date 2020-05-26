using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    public class DefaultSessionBehaviour : ISessionBehaviour
    {
        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber,
            IFakeUser user)
        {
            var gpUserSession = new FakeUserSession
            {
                Id = Guid.NewGuid(),
                NhsNumber = nhsNumber,
                OdsCode = odsCode,
                Name = user.Name
            };

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