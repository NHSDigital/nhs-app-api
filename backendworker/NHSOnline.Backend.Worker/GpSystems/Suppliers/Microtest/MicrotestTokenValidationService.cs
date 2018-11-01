using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest
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
            // TODO
            // Validate when we know connection token format.
            return true;
        }
    }
}
