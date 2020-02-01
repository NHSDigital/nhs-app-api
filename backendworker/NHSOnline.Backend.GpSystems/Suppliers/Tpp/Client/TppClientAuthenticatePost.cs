using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientAuthenticatePost : ITppClientRequest<Authenticate, AuthenticateReply>
    {
        private readonly TppConfigurationSettings _config;
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientAuthenticatePost(
            TppConfigurationSettings config,
            TppClientRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
            _config = config;
        }

        public async Task<TppApiObjectResponse<AuthenticateReply>> Post(Authenticate authenticate)
        {
            authenticate.Application = new Application
            {
                Name = _config.ApplicationName,
                Version = _config.ApplicationVersion,
                ProviderId = authenticate.ProviderId,
                DeviceType = _config.ApplicationDeviceType
            };

            return await _requestExecutor.Post<AuthenticateReply>(
                requestBuilder => requestBuilder.Model(authenticate));
        }
    }
}