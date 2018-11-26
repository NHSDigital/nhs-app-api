using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Logging;

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
            _logger.LogEnter();
            if (string.IsNullOrEmpty(connectionToken))
            {
                return false;
            }
            
            try
            {
                var auth = connectionToken.DeserializeJson<Authenticate>();

                var validator = new ValidateAndLog(_logger)
                    .IsNotNullOrWhitespace(auth?.AccountId, nameof(auth.AccountId))
                    .IsNotNullOrWhitespace(auth?.Passphrase, nameof(auth.Passphrase))
                    .IsNotNullOrWhitespace(auth?.ProviderId, nameof(auth.ProviderId));

                return validator.IsValid();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to deserialize Im1TokenToken");
                return false;
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}