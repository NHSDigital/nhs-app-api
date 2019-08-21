using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Im1Connection;
using PatientIm1ConnectionResponse = NHSOnline.Backend.CidApi.Areas.Im1Connection.Models.PatientIm1ConnectionResponse;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    internal class Im1ConnectionVerifyResultVisitor : IIm1ConnectionVerifyResultVisitor<IActionResult>
    {
        public IActionResult Visit(Im1ConnectionVerifyResult.Success result)
        {
            return new OkObjectResult(CreateResponse(result.Response));
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.NotFound result)
        {
            return new NotFoundResult();
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.Forbidden result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        private static PatientIm1ConnectionResponse CreateResponse(GpSystems.Im1Connection.Models.PatientIm1ConnectionResponse response)
            => new PatientIm1ConnectionResponse
            {
                ConnectionToken = response.ConnectionToken,
                NhsNumbers = response.NhsNumbers
            };
    }
}