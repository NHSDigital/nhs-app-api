using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public interface IGpSessionCreator
    {
        Task<GpSessionCreateResult> CreateGpSession(CitizenIdSessionResult citizenIdUserSession, Supplier supplier);

        Task<GpSessionRecreateResult> RecreateGpSession(P9UserSession userSession, Supplier supplier);
    }
}