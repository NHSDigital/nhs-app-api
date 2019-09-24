using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    internal class DeviceRegistrationResultVisitor : IDeviceRegistrationResultVisitor<IActionResult>
    {
        private readonly RegisterDeviceRequest _initialRequest;

        public DeviceRegistrationResultVisitor(RegisterDeviceRequest initialRequest)
        {
            _initialRequest = initialRequest;
        }

        public IActionResult Visit(DeviceRegistrationResult.Created result)
        {
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

        public IActionResult Visit(DeviceRegistrationResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(DeviceRegistrationResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}