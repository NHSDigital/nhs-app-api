using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.UsersApi.Azure
{
    internal class RegistrationResultVisitor : IRegistrationResultVisitor<IActionResult>
    {
        private readonly ILogger _logger;

        public RegistrationResultVisitor(ILogger logger)
        {
            _logger = logger;
        }

        public IActionResult Visit(RegistrationResult.Success result)
        {
            _logger.LogInformation("Registration success");
            return new StatusCodeResult(StatusCodes.Status201Created);
        }

        public IActionResult Visit(RegistrationResult.Failure result)
        {
            _logger.LogError("Registration failed");
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
    }
}