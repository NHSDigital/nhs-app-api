using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    [FakeGpAreaBehaviour(Behaviour.BadRequest)]
    public class BadRequestSessionAreaBehaviour : ISessionAreaBehaviour
    {
        public Task<GpSessionCreateResult> Create(
            string connectionToken,
            string odsCode,
            string nhsNumber,
            FakeUser user
        )
        {
            return Task.FromResult<GpSessionCreateResult>(
                new GpSessionCreateResult.BadRequest(nameof(BadRequestSessionAreaBehaviour))
            );
        }

        public Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            return Task.FromResult<SessionLogoffResult>(new SessionLogoffResult.BadGateway());
        }

        public Task<GpSessionRecreateResult> Recreate(
            string connectionToken,
            string odsCode,
            string nhsNumber,
            string patientId)
        {
            return Task.FromResult<GpSessionRecreateResult>(
                new GpSessionRecreateResult.Failure()
            );
        }
    }
}