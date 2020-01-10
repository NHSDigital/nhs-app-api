using NHSOnline.Backend.PfsApi.Areas.Configuration;

namespace NHSOnline.Backend.PfsApi.Configuration
{
    public interface IConfigurationService
    {
        GetConfigurationResultV2 GetConfiguration();
    }
}