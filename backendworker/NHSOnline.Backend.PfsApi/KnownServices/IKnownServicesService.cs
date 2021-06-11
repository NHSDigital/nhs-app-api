using NHSOnline.Backend.PfsApi.Areas.KnownServices;

namespace NHSOnline.Backend.PfsApi.KnownServices
{
    public interface IKnownServicesService
    {
        GetKnownServicesV3Result GetConfiguration();
    }
}