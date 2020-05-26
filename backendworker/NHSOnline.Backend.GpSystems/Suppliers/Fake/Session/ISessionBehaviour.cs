using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    public interface ISessionBehaviour
    {
        Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber,
            IFakeUser user);

        Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession);

        Task<GpSessionRecreateResult> Recreate(string connectionToken, string odsCode, string nhsNumber,
            string patientId);
    }
}