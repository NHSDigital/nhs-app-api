using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class DeleteDeviceResultVisitor : IDeleteDeviceResultVisitor<IActionResult>
    {
        public IActionResult Visit(DeleteDeviceResult.Success result)
        {
            return new StatusCodeResult(StatusCodes.Status204NoContent);
        }

        public IActionResult Visit(DeleteDeviceResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(DeleteDeviceResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}