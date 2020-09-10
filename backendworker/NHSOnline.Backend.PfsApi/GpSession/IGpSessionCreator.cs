using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public interface IGpSessionCreator
    {
        Task<GpUserSession> CreateGpSession(CitizenIdSessionResult citizenIdUserSession, Supplier supplier);

        Task<GpSessionRecreateResult> RecreateGpSession(P9UserSession userSession, Supplier supplier);
    }
}