using NHSOnline.Backend.Support;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Session
{
    public interface ISessionService
    {
        Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber);

        Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession);
        
        Task<GpSessionRecreateResult> Recreate(string connectionToken, string odsCode, string nhsNumber, string patientId);

    }
}
