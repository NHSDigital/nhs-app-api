using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;


namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public interface IGpSessionManager
    {
        Task<GpSessionCreateResult> CreateSession(IGpSessionCreateArgs args);

        Task<RetrieveSessionResult> RetrieveSession(string sessionId, StringValues csrfToken);

        Task<RecreateSessionResult> RecreateSession(string patientId);

        Task<CloseSessionResult> CloseAndDeleteSession(P9UserSession userSession);

        Task<CloseSessionResult> CloseSession(GpUserSession gpUserSession);

    }
}