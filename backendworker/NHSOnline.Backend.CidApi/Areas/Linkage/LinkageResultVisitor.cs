using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.CidApi.Areas.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;

namespace NHSOnline.Backend.CidApi.Areas.Linkage
{
    public class LinkageResultVisitor : ILinkageResultVisitor<Task<IActionResult>>
    {
        public LinkageResultVisitor(ILogger logger)
        { 
            Logger = logger;
        }
        private ILogger Logger { get; }

        public async Task<IActionResult> Visit(LinkageResult.ErrorCase result)
        {
            Logger.LogInformation("Linkage resulted in an error which was mapped");
            var statusCodeResult = Im1ConnectionV1ErrorCodeMapper.Map(result.ErrorCode);
            return await Task.FromResult(new StatusCodeResult(statusCodeResult));
        }

        public async Task<IActionResult> Visit(LinkageResult.SupplierSystemUnavailable result)
        {
            Logger.LogInformation("Linkage resulted in a supplier system unavailable error");
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status503ServiceUnavailable));
        }

        public async Task<IActionResult> Visit(LinkageResult.InternalServerError result)
        {
            Logger.LogError("Linkage resulted in an internal server error");
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status500InternalServerError));
        }

        public async Task<IActionResult> Visit(LinkageResult.SuccessfullyRetrieved result)
        {
            Logger.LogInformation("Linkage resulted in a success");
            return await Task.FromResult(new OkObjectResult(result.Response));
        }

        public async Task<IActionResult> Visit(LinkageResult.SuccessfullyCreated result)
        {
            Logger.LogInformation("Linkage resulted in a success");
            return await Task.FromResult(new CreatedResult(string.Empty, result.Response));
        }

        public async Task<IActionResult> Visit(LinkageResult.SuccessfullyRetrievedAlreadyExists result)
        {
            Logger.LogInformation("Linkage resulted in a success");
            return await Task.FromResult(new OkObjectResult(result.Response));
        }

        public async Task<IActionResult> Visit(LinkageResult.NotFound result)
        {
            Logger.LogInformation("Linkage resulted in a not found error");
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status404NotFound));
        }

        public async Task<IActionResult> Visit(LinkageResult.UnmappedErrorWithStatusCode result)
        {
            Logger.LogError("Linkage resulted in an error which was unmappable");
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status502BadGateway));
        }
    }
}