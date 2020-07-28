using NHSOnline.Backend.GpSystems.SessionManager;

namespace NHSOnline.Backend.PfsApi.GpSession.Models
{
    internal sealed class GpSessionRecreateArgs : IGpSessionCreateArgs
    {
        public GpSessionRecreateArgs(string im1ConnectionToken, string odsCode, string nhsNumber)
        {
            Im1ConnectionToken = im1ConnectionToken;
            OdsCode = odsCode;
            NhsNumber = nhsNumber;
        }

        public string Im1ConnectionToken { get; }
        public string OdsCode { get; }
        public string NhsNumber { get; }
    }
}