using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.CidApi.Areas.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.CidApi.Areas.Linkage
{
    public class LinkageV2ResultVisitor : ILinkageResultVisitor<Task<IActionResult>>
    {
        public LinkageV2ResultVisitor(IIm1ConnectionErrorCodes errorCodes, Supplier gpSystem, ILogger logger)
        {
            ErrorCodes = errorCodes;
            GpSystem = gpSystem;
            Logger = logger;
        }

        private IIm1ConnectionErrorCodes ErrorCodes { get; }
        private Supplier GpSystem { get; }
        private ILogger Logger { get; }

        public async Task<IActionResult> Visit(LinkageResult.ErrorCase result)
        {
            Logger.LogInformation("Linkage resulted in an error which was mapped");
            var response = ErrorCodes.GetAndLogErrorResponse(result.ErrorCode, GpSystem, Logger);
            var externalErrorCode = ErrorCodes.GetExternalCode(result.ErrorCode);
            var statusCodeResult = Im1ConnectionV2ErrorCodeToStatusCodeMapper.Map(externalErrorCode);

            return await Task.FromResult(new ObjectResult(response)
            {
                StatusCode = statusCodeResult
            });
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
            return await Task.FromResult(new OkObjectResult(result.Response));
        }

        public async Task<IActionResult> Visit(LinkageResult.SuccessfullyRetrievedAlreadyExists result)
        {
            Logger.LogInformation("Linkage resulted in a success");
            return await Task.FromResult(new OkObjectResult(result.Response));
        }

        public async Task<IActionResult> Visit(LinkageResult.NotFound result)
        {
            Logger.LogInformation("Linkage resulted in a not found error");
            var response = ErrorCodes.GetAndLogErrorResponse(result.ErrorCode, GpSystem, Logger);
            return await Task.FromResult(new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status404NotFound

            });
        }

        public async Task<IActionResult> Visit(LinkageResult.UnmappedErrorWithStatusCode result)
        {
            Logger.LogError("Linkage resulted in an error which was unmappable");
            var response = ErrorCodes.GetAndLogErrorResponse(result.ErrorCode, GpSystem, Logger);
            return await Task.FromResult(new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status502BadGateway
            });
        }
    }
}