using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.CitizenId;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    internal sealed class GpSessionCreateArgs : IGpSessionCreateArgs
    {
        public GpSessionCreateArgs(IGpSystem gpSystem, CitizenIdSessionResult citizenIdSessionResult)
        {
            GpSystem = gpSystem;
            Im1ConnectionToken = citizenIdSessionResult.Im1ConnectionToken;
            OdsCode = citizenIdSessionResult.Session.OdsCode;
            NhsNumber = citizenIdSessionResult.NhsNumber;
        }

        public IGpSystem GpSystem { get; }
        public string Im1ConnectionToken { get; }
        public string OdsCode { get; }
        public string NhsNumber { get; }
    }
}