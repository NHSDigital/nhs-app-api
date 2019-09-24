using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    internal class SearchDeviceResultVisitor : ISearchDeviceResultVisitor<IActionResult>
    {
        public IActionResult Visit(SearchDeviceResult.Found result)
        {
            return new StatusCodeResult(StatusCodes.Status204NoContent);
        }

        public IActionResult Visit(SearchDeviceResult.NotFound result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(SearchDeviceResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(SearchDeviceResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}