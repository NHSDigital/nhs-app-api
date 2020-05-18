using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public interface IGpSessionManager
    {
        Task<GpSessionCreateResult> CreateSession(IGpSessionCreateArgs args);

        Task<RetrieveSessionResult> RetrieveSession(string sessionId, StringValues csrfToken);

        Task<RecreateSessionResult> RecreateSession(string patientId);

        Task<CloseSessionResult> CloseSession(GpUserSession gpUserSession);
    }
}