using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.CitizenId;

namespace NHSOnline.Backend.PfsApi.GpSession.Models
{
    internal sealed class GpSessionCreateArgs : IGpSessionCreateArgs
    {
        public GpSessionCreateArgs(CitizenIdSessionResult citizenIdSessionResult)
        {
            Im1ConnectionToken = citizenIdSessionResult.Im1ConnectionToken;
            OdsCode = citizenIdSessionResult.Session.OdsCode;
            NhsNumber = citizenIdSessionResult.NhsNumber;
        }

        public string Im1ConnectionToken { get; }
        public string OdsCode { get; }
        public string NhsNumber { get; }
    }
}