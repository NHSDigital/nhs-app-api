using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    [FakeGpAreaBehaviour(Behaviour.Timeout)]
    public class TimeoutSessionAreaBehaviour : ISessionAreaBehaviour
    {
        public Task<GpSessionCreateResult> Create(
            string connectionToken,
            string odsCode,
            string nhsNumber,
            FakeUser user
        )
        {
            return Task.FromResult<GpSessionCreateResult>(
                new GpSessionCreateResult.Timeout(nameof(TimeoutSessionAreaBehaviour))
            );
        }

        public Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            throw new NhsTimeoutException("timeout");
        }

        public Task<GpSessionRecreateResult> Recreate(
            string connectionToken,
            string odsCode,
            string nhsNumber,
            string patientId)
        {
            throw new NhsTimeoutException("timeout");
        }
    }
}