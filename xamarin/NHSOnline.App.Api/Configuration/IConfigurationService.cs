using System.Threading.Tasks;

namespace NHSOnline.App.Api.Configuration
{
    public interface IConfigurationService
    {
        public Task<GetConfigurationResult> GetConfiguration();
    }
}