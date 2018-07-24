using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppTokenValidationService : ITokenValidationService
    {
        private readonly ILogger<TppTokenValidationService> _logger;

        public TppTokenValidationService(ILogger<TppTokenValidationService> logger)
        {
            _logger = logger;
        }
        public bool IsValidConnectionTokenFormat(string connectionToken)
        {   
            if (string.IsNullOrEmpty(connectionToken))
            {
                return false;
            }
            
            try
            {
                var auth = connectionToken.DeserializeJson<Authenticate>();
                return !string.IsNullOrEmpty(auth?.AccountId) && !string.IsNullOrEmpty(auth.Passphrase);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to deserialize Im1TokenToken");
                return false;
            }
        }
    }
}