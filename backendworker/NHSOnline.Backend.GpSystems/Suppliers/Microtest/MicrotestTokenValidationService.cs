using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Im1Connection;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestTokenValidationService: ITokenValidationService
    {
        
        private readonly ILogger<MicrotestTokenValidationService> _logger;

        public MicrotestTokenValidationService(ILogger<MicrotestTokenValidationService> logger)
        {
            _logger = logger;
        }
        
        public bool IsValidConnectionTokenFormat(string connectionToken)
        {
            _logger.LogEnter();
            try
            {
                var token = connectionToken.DeserializeJson<MicrotestConnectionToken>();
                _logger.LogDebug("Successfully deserialized Im1 Connection Token");
                return new ValidateAndLog(_logger)
                    .IsNotNullOrWhitespace(token?.NhsNumber, nameof(token.NhsNumber))
                    .IsValid();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to deserialize Im1 Connection Token");
                return false;
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
