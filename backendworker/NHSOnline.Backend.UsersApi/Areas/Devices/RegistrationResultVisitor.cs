using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
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

        public IActionResult Visit(RegistrationResult.BadGateway result)
        {
            _logger.LogError("Registration failed");
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(RegistrationResult.InternalServerError result)
        {
            _logger.LogError("Registration failed");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}