using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public interface ICreateGpSessionOnDemandRequest : ICreateSessionRequest
    {
        P9UserSession UserSession { get; }
    }
}