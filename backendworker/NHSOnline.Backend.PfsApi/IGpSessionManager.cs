using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.CitizenId;

namespace NHSOnline.Backend.PfsApi
{
    public interface IGpSessionManager
    {
        Task<CreateSessionResult> CreateSession(IGpSystem gpSystem, CitizenIdSessionResult citizenIdSessionResult);

        Task<RetrieveSessionResult> RetrieveSession(string sessionId, StringValues csrfToken);

        Task<RecreateSessionResult> RecreateSession(string patientId);
    }
}