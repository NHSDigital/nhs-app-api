using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppHttpClientHandler : HttpClientCertificateHandler<TppHttpClientHandler>
    {
        public TppHttpClientHandler(IConfiguration configuration, ILogger<TppHttpClientHandler> logger) :
            base(configuration, logger)
        {
            var path = configuration.GetOrWarn("TPP_CERTIFICATE_PATH", logger);
            var password = configuration.GetOrWarn("TPP_CERTIFICATE_PASSWORD", logger);
            logger.LogInformation("TPP_CERTIFICATE_PATH: {path}", path);

            if (ValidateParameters(path, password))
            {
                AddCertificate(path, password);
            }
            else
            {
                logger.LogWarning("Could not add TPP client certificate due to missing certificate path or password.");
            }
        }
    }
}
