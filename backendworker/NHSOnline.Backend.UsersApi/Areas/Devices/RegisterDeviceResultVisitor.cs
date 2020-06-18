using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    internal class RegisterDeviceResultVisitor : IRegisterDeviceResultVisitor<IActionResult>
    {
        private readonly RegisterDeviceRequest _initialRequest;

        public RegisterDeviceResultVisitor(RegisterDeviceRequest initialRequest)
        {
            _initialRequest = initialRequest;
        }

        public IActionResult Visit(RegisterDeviceResult.Created result)
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

        public IActionResult Visit(RegisterDeviceResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(RegisterDeviceResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}