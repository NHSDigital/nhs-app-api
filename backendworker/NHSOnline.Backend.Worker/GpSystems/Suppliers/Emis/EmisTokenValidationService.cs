using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisTokenValidationService: ITokenValidationService
    {
        private readonly ILogger<EmisTokenValidationService> _logger;

        public EmisTokenValidationService(ILogger<EmisTokenValidationService> logger)
        {
            _logger = logger;
        }

        public bool IsValidConnectionTokenFormat(string connectionToken)
        {
            if (string.IsNullOrWhiteSpace(connectionToken))
            {
                return false;
            }

            if (IsGuid(connectionToken)) return true;

            try
            {
                var token = connectionToken.DeserializeJson<EmisConnectionToken>();

                var validator = new ValidateAndLog(_logger)
                    .IsNotNullOrWhitespace(token?.Im1CacheKey, nameof(token.Im1CacheKey))
                    .IsNotNullOrWhitespace(token?.AccessIdentityGuid, nameof(token.AccessIdentityGuid));

                return validator.IsValid() && IsGuid(token?.AccessIdentityGuid);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to deserialize Im1 Connection Token");
                return false;
            }
        }

        private static bool IsGuid(string connectionToken)
        {
            return Guid.TryParse(connectionToken, out _);
        }
    }
}
