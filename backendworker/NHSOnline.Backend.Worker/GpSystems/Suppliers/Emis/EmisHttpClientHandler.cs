using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisHttpClientHandler : HttpClientCertificateHandler<EmisHttpClientHandler>
    {
        public EmisHttpClientHandler(IConfiguration configuration, ILogger<EmisHttpClientHandler> logger) :
            base(configuration, logger)
        {
            var path = configuration.GetOrWarn("EMIS_CERTIFICATE_PATH", logger);
            logger.LogInformation("EMIS_CERTIFICATE_PATH: {path}", path);

            if (ValidateParameters(path))
            {
                AddCertificate(path);
            }
            else
            {
                logger.LogWarning("Could not add Emis client certificate due to missing certificate path.");
            }
        }

        
    }
}