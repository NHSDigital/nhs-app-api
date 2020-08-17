using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class DeleteRegistrationResultVisitor : IDeleteRegistrationResultVisitor<Task<IActionResult>>
    {
        private readonly IMetricLogger _metricLogger;

        public DeleteRegistrationResultVisitor(IMetricLogger metricLogger)
        {
            _metricLogger = metricLogger;
        }

        public async Task<IActionResult> Visit(DeleteRegistrationResult.Success result)
        {
            await _metricLogger.NotificationsDisabled();
            return new StatusCodeResult(StatusCodes.Status204NoContent);
        }

        public async Task<IActionResult> Visit(DeleteRegistrationResult.NotFound result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status404NotFound));
        }

        public async Task<IActionResult> Visit(DeleteRegistrationResult.BadGateway result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status502BadGateway));
        }

        public async Task<IActionResult> Visit(DeleteRegistrationResult.InternalServerError result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status500InternalServerError));
        }
    }
}