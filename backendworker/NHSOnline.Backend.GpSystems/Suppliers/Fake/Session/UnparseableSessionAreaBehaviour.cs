using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    [FakeGpAreaBehaviour(Behaviour.Unparseable)]
    public class UnparseableSessionAreaBehaviour : ISessionAreaBehaviour
    {
        public Task<GpSessionCreateResult> Create(
            string connectionToken,
            string odsCode,
            string nhsNumber,
            FakeUser user
        )
        {
            return Task.FromResult<GpSessionCreateResult>(
                new GpSessionCreateResult.Unparseable(nameof(UnparseableSessionAreaBehaviour))
            );
        }

        public Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            throw new NhsUnparsableException("unparseable");
        }

        public Task<GpSessionRecreateResult> Recreate(
            string connectionToken,
            string odsCode,
            string nhsNumber,
            string patientId)
        {
            throw new NhsUnparsableException("unparseable");
        }
    }
}