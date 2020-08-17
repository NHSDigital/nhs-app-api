using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    internal class RegisterDeviceResultVisitor : IRegisterDeviceResultVisitor<Task<IActionResult>>
    {
        private readonly RegisterDeviceRequest _initialRequest;
        private readonly IMetricLogger _metricLogger;

        public RegisterDeviceResultVisitor(RegisterDeviceRequest initialRequest, IMetricLogger metricLogger)
        {
            _initialRequest = initialRequest;
            _metricLogger = metricLogger;
        }

        public async Task<IActionResult> Visit(RegisterDeviceResult.Created result)
        {
            var device = new Device
            {
                DeviceId = result.UserDevice.DeviceId,
                DeviceType = _initialRequest.DeviceType
            };

            await _metricLogger.NotificationsEnabled();

            return new ObjectResult(device)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

        public async Task<IActionResult> Visit(RegisterDeviceResult.BadGateway result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status502BadGateway));
        }

        public async Task<IActionResult> Visit(RegisterDeviceResult.InternalServerError result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status500InternalServerError));
        }
    }
}