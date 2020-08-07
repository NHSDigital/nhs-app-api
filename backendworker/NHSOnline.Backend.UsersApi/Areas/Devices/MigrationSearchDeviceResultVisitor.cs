using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class MigrationSearchDeviceResultVisitor : ISearchDeviceResultVisitor<IActionResult>
    {
        public IActionResult Visit(SearchDeviceResult.Found result) => throw new NotImplementedException();

        public IActionResult Visit(SearchDeviceResult.FoundMany result) => throw new NotImplementedException();

        public IActionResult Visit(SearchDeviceResult.NotFound result) => throw new NotImplementedException();

        public IActionResult Visit(SearchDeviceResult.BadGateway result)
            => new StatusCodeResult(StatusCodes.Status502BadGateway);

        public IActionResult Visit(SearchDeviceResult.InternalServerError result)
            => new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}
