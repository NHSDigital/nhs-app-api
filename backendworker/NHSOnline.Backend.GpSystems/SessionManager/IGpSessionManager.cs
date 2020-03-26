using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using NHSOnline.Backend.GpSystems.SessionManager.Model;
using NHSOnline.Backend.Support;


namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public interface IGpSessionManager
    {
        Task<CreateSessionResult> CreateSession(IGpSystem gpSystem, GpSessionManagerCitizenIdSessionResult citizenIdSessionResult);

        Task<RetrieveSessionResult> RetrieveSession(string sessionId, StringValues csrfToken);

        Task<RecreateSessionResult> RecreateSession(string patientId);

        void LogoffSession(GpUserSession gpUserSession);
    }
}