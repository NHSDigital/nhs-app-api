using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    [FakeGpArea("Session")]
    public interface ISessionAreaBehaviour
    {
        Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber,
            FakeUser user);

        Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession);

        Task<GpSessionRecreateResult> Recreate(string connectionToken, string odsCode, string nhsNumber,
            string patientId);
    }
}