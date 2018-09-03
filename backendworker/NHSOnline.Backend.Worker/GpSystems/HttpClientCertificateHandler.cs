using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems
{
    public abstract class HttpClientCertificateHandler<T> : HttpClientHandler
    {
        private readonly ILogger<T> _logger;

        protected HttpClientCertificateHandler(IConfiguration configuration, ILogger<T> logger)
        {
            _logger = logger;
            
            if (!"Production".Equals(configuration["ASPNETCORE_ENVIRONMENT"], StringComparison.OrdinalIgnoreCase))
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }
        }

        protected bool ValidateParameters(string path, string password)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path) && !string.IsNullOrEmpty(password);
        }
        protected bool ValidateParameters(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }
        
        protected void AddCertificate(string path, string password)
        {
            try
            {
                ClientCertificates.Add(new X509Certificate2(path, password));
            }
            catch (CryptographicException e)
            {
                _logger.LogError(e.Message);
            }        
        }
        protected void AddCertificate(string path)
        {
            try
            {
                ClientCertificates.Add(new X509Certificate(path));
            }
            catch (CryptographicException e)
            {
                _logger.LogError(e.Message);
            }        
        }
    }
}