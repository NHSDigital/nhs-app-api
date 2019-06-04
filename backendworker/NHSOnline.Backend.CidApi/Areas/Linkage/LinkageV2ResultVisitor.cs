using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.CidApi.Areas.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;

namespace NHSOnline.Backend.CidApi.Areas.Linkage
{
    public class LinkageV2ResultVisitor : ILinkageResultVisitor<Task<IActionResult>>
    {
        public LinkageV2ResultVisitor(IIm1ConnectionErrorCodes errorCodes)
        {
            ErrorCodes = errorCodes;
        }

        private IIm1ConnectionErrorCodes ErrorCodes { get; }

        public async Task<IActionResult> Visit(LinkageResult.ErrorCase result)
        {
            var response = ErrorCodes.GetErrorResponse(result.ErrorCode);
            var statusCodeResult = Im1ConnectionV2ErrorCodeMapper.Map(result.ErrorCode);

            return await Task.FromResult(new ObjectResult(response)
            {
                StatusCode = statusCodeResult
            });
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
            return await Task.FromResult(new OkObjectResult(result.Response));
        }

        public async Task<IActionResult> Visit(LinkageResult.SuccessfullyRetrievedAlreadyExists result)
        {
            return await Task.FromResult(new OkObjectResult(result.Response));
        }

        public async Task<IActionResult> Visit(LinkageResult.NotFound result)
        {
            var response = ErrorCodes.GetErrorResponse(result.ErrorCode);
            return await Task.FromResult(new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status404NotFound
            });
        }

        public async Task<IActionResult> Visit(LinkageResult.BadRequest result)
        {
            var response = ErrorCodes.GetErrorResponse(result.ErrorCode);
            return await Task.FromResult(new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        public async Task<IActionResult> Visit(LinkageResult.Conflict result)
        {
            var response = ErrorCodes.GetErrorResponse(result.ErrorCode);

            return await Task.FromResult(new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status409Conflict
            });
        }

        public async Task<IActionResult> Visit(LinkageResult.Forbidden result)
        {
            var response = ErrorCodes.GetErrorResponse(result.ErrorCode);
            return await Task.FromResult(new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status403Forbidden
            });
        }

        public async Task<IActionResult> Visit(LinkageResult.UnknownError result)
        {
            var response = ErrorCodes.GetErrorResponse(result.ErrorCode);
            return await Task.FromResult(new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status502BadGateway
            });
        }
    }
}