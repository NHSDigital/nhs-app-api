using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.LoggerApi.Areas.Logging.Models;
using NHSOnline.Backend.LoggerApi.Logging;

namespace NHSOnline.Backend.LoggerApi.Areas.Logging
{
    [Route("log")]
    public class LoggingController : Controller
    {
        private readonly ILogger<LoggingController> _logger;
        private readonly ILoggingService _loggingService;

        public LoggingController(
            ILogger<LoggingController> logger, ILoggingService loggingService)
        {
            _logger = logger;
            _loggingService = loggingService;
        }

        [HttpPost, AllowAnonymous]
        public IActionResult Post([FromBody] CreateLogRequest model)
        {
            var createLogRequestValidator = new CreateLogRequestValidator(_logger);

            if (!createLogRequestValidator.ValidateAndSanitize(model))
            {
                return new BadRequestResult();
            }

            try
            {
                _loggingService.LogMessage(model);
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error calling {nameof(_loggingService.LogMessage)}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
