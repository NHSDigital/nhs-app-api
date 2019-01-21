using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class VisionTokenValidationService : ITokenValidationService
    {
        private readonly ILogger<VisionTokenValidationService> _logger;

        public VisionTokenValidationService(ILogger<VisionTokenValidationService> logger)
        {
            _logger = logger;
        }
            
        public bool IsValidConnectionTokenFormat(string connectionToken)
        {
            _logger.LogEnter();
            if (string.IsNullOrEmpty(connectionToken))
            {
                return false;
            }

            try
            {
                var token = connectionToken.DeserializeJson<Im1ConnectionToken>();
                return !string.IsNullOrEmpty(token?.RosuAccountId) && !string.IsNullOrEmpty(token.ApiKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to deserialize connection token '{0}'.", connectionToken);
                return false;
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}