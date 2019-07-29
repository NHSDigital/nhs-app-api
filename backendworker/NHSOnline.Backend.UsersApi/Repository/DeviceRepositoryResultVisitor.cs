using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Repository
{
    internal class DeviceRepositoryResultVisitor : IDeviceRepositoryResultVisitor<IActionResult>
    {
        private readonly RegisterDeviceRequest _initialRequest;
        private readonly ILogger _logger;

        public DeviceRepositoryResultVisitor(RegisterDeviceRequest initialRequest, ILogger logger)
        {
            _initialRequest = initialRequest;
            _logger = logger;
        }

        public IActionResult Visit(DeviceRepositoryResult.Created result)
        {
            _logger.LogInformation($"Device registered: Id '{result.UserDevice.DeviceId}', Type '{_initialRequest.DeviceType}'");
            var device = new Device
            {
                DeviceId = result.UserDevice.DeviceId,
                DeviceType = _initialRequest.DeviceType
            };

            return new ObjectResult(device)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

        public IActionResult Visit(DeviceRepositoryResult.Failure result)
        {
            _logger.LogError("Failure to register device.");
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
    }
}