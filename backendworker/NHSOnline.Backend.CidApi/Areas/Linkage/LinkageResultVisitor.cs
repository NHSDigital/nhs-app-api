using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.CidApi.Areas.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;

namespace NHSOnline.Backend.CidApi.Areas.Linkage
{
    public class LinkageResultVisitor : ILinkageResultVisitor<Task<IActionResult>>
    {
        public async Task<IActionResult> Visit(LinkageResult.ErrorCase result)
        {
            var statusCodeResult = Im1ConnectionV1ErrorCodeMapper.Map(result.ErrorCode);
            return await Task.FromResult(new StatusCodeResult(statusCodeResult));
        }

        public async Task<IActionResult> Visit(LinkageResult.SupplierSystemUnavailable result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status503ServiceUnavailable));
        }

        public async Task<IActionResult> Visit(LinkageResult.InternalServerError result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status500InternalServerError));
        }

        public async Task<IActionResult> Visit(LinkageResult.SuccessfullyRetrieved result)
        {
            return await Task.FromResult(new OkObjectResult(result.Response));
        }

        public async Task<IActionResult> Visit(LinkageResult.SuccessfullyCreated result)
        {
            return await Task.FromResult(new CreatedResult(string.Empty, result.Response));
        }

        public async Task<IActionResult> Visit(LinkageResult.SuccessfullyRetrievedAlreadyExists result)
        {
            return await Task.FromResult(new OkObjectResult(result.Response));
        }

        public async Task<IActionResult> Visit(LinkageResult.NotFound result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status404NotFound));
        }

        public async Task<IActionResult> Visit(LinkageResult.BadRequest result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status400BadRequest));
        }

        public async Task<IActionResult> Visit(LinkageResult.Conflict result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status409Conflict));
        }

        public async Task<IActionResult> Visit(LinkageResult.Forbidden result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status403Forbidden));
        }

        public async Task<IActionResult> Visit(LinkageResult.UnknownError result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status502BadGateway));
        }
    }
}