using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public interface ITppHttpClientHandler
    {
    }

    public class TppHttpClientHandler : HttpClientHandler, ITppHttpClientHandler
    {
        public TppHttpClientHandler(IConfiguration configuration, ILogger<TppHttpClientHandler> logger)
        {
            if (configuration["ASPNETCORE_ENVIRONMENT"] != "Production")
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            var path = configuration.GetOrWarn("TPP_CERTIFICATE_PATH", logger);
            var password = configuration.GetOrWarn("TPP_CERTIFICATE_PASSWORD", logger);
            logger.LogInformation("TPP_CERTIFICATE_PATH: {path}", path);

            if (!string.IsNullOrEmpty(path) && File.Exists(path) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    ClientCertificates.Add(new X509Certificate2(path, password));
                }
                catch (CryptographicException e)
                {
                    logger.LogError(e.Message);
                }        
            }
            else
            {
                logger.LogWarning("Could not add TPP client certificate due to missing certificate path or password.");
            }
        }
    }
}