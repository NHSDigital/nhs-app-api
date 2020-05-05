using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.AssertedLoginIdentity
{
    public class CreateJwtResultVisitor : ICreateJwtResultVisitor<Task<IActionResult>>
    {
        private readonly ILogger _logger;
        private readonly IMetricLogger _metricLogger;
        private readonly CreateJwtRequest _request;

        public CreateJwtResultVisitor(
            ILogger logger,
            IMetricLogger metricLogger,
            CreateJwtRequest request)
        {
            _logger = logger;
            _metricLogger = metricLogger;
            _request = request;
        }

        public async Task<IActionResult> Visit(CreateJwtResult.Success result)
        {
            LogAssertedLoginIdentity();
            await LogMetric();

            return new CreatedResult(string.Empty, result.Response);
        }

        public Task<IActionResult> Visit(CreateJwtResult.InternalServerError result)
        {
            IActionResult statusCodeResult = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return Task.FromResult(statusCodeResult);
        }

        private async Task LogMetric()
        {
            switch (_request.Action)
            {
                case "UpliftStarted":
                    await _metricLogger.UpliftStarted();
                    break;
                case { } unknownAction:
                    _logger.LogDebug("No metric logging for action {Action}", unknownAction);
                    break;
            }
        }

        private void LogAssertedLoginIdentity()
        {
            var kvp = new Dictionary<string, string>
            {
                { "ProviderId", _request.ProviderId },
                { "ProviderName", _request.ProviderName },
                { "JumpOffId", _request.JumpOffId },
                { "IntendedRelyingPartyUrl", _request.IntendedRelyingPartyUrl },
            };
            _logger.LogInformationKeyValuePairs("Created Asserted Login Identity", kvp);
        }
    }
}