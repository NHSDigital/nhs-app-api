using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Models.Patient;
using NHSOnline.Backend.Worker.Ods;
using NHSOnline.Backend.Worker.Suppliers;

namespace NHSOnline.Backend.Worker.Controllers.Patient
{
    [Route("/patient/im1connection")]
    public class Im1ConnectionController : Controller
    {
        private readonly IOdsCodeLookup _odsCodeLookup;
        private readonly ISystemProviderFactory _systemProviderFactory;

        public Im1ConnectionController(IOdsCodeLookup odsCodeLookup, ISystemProviderFactory systemProviderFactory)
        {
            _odsCodeLookup = odsCodeLookup ?? throw new ArgumentNullException(nameof(odsCodeLookup));
            _systemProviderFactory =
                systemProviderFactory ?? throw new ArgumentNullException(nameof(systemProviderFactory));
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromHeader(Name = Headers.ConnectionToken)]
            string connectionToken,
            [FromHeader(Name = Headers.OdsCode)] string odsCode
        )
        {
            if (string.IsNullOrEmpty(connectionToken) || string.IsNullOrEmpty(odsCode))
            {
                return BadRequest();
            }

            if (!Regex.IsMatch(odsCode, OdsCodeFormats.GpPracticeEnglandWales))
            {
                return BadRequest();
            }

            ISystemProvider systemProvider;

            try
            {
                var supplier = await _odsCodeLookup.LookupSupplier(odsCode);
                systemProvider = _systemProviderFactory.CreateSystemProvider(supplier);
            }
            catch (OdsCodeLookupException)
            {
                return NotFound();
            }
            catch (UnknownSupplierException)
            {
                return NotFound();
            }

            var im1ConnectionService = systemProvider.GetIm1ConnectionService();
            var verifyResult = await im1ConnectionService.Verify(connectionToken, odsCode);

            return verifyResult.Accept(new Im1ConnectionVerifyResultVisitor());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PatientIm1ConnectionRequest model)
        {
            ISystemProvider systemProvider;

            try
            {
                var supplier = await _odsCodeLookup.LookupSupplier(model.OdsCode);
                systemProvider = _systemProviderFactory.CreateSystemProvider(supplier);
            }
            catch (OdsCodeLookupException)
            {
                return NotFound();
            }
            catch (UnknownSupplierException)
            {
                return NotFound();
            }

            var im1ConnectionService = systemProvider.GetIm1ConnectionService();
            var registerResult = await im1ConnectionService.Register(model);

            return registerResult.Accept(new Im1ConnectionRegisterResultVisitor(Request));
        }

        private class Im1ConnectionVerifyResultVisitor : Im1ConnectionVerifyResult.IIm1ConnectionVerifyResultVisitor<IActionResult>
        {
            public IActionResult Visit(Im1ConnectionVerifyResult.SuccessfullyVerified result)
            {
                return new OkObjectResult(result.Response);
            }

            public IActionResult Visit(Im1ConnectionVerifyResult.InsufficientPermissions result)
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            public IActionResult Visit(Im1ConnectionVerifyResult.NotFound result)
            {
                return new NotFoundResult();
            }

            public IActionResult Visit(Im1ConnectionVerifyResult.SupplierSystemUnavailable result)
            {
                return new StatusCodeResult(StatusCodes.Status502BadGateway);
            }
        }

        private class Im1ConnectionRegisterResultVisitor : Im1ConnectionRegisterResult.IIm1ConnectionRegisterResultVisitor<IActionResult>
        {
            private HttpRequest Request { get; }

            public Im1ConnectionRegisterResultVisitor(HttpRequest request)
            {
                Request = request;
            }

            public IActionResult Visit(Im1ConnectionRegisterResult.SuccessfullyRegistered result)
            {
                return new CreatedResult(Request.GetDisplayUrl(), result.Response);
            }

            public IActionResult Visit(Im1ConnectionRegisterResult.InsufficientPermissions result)
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            public IActionResult Visit(Im1ConnectionRegisterResult.NotFound result)
            {
                return new NotFoundResult();
            }

            public IActionResult Visit(Im1ConnectionRegisterResult.AccountAlreadyExists result)
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            public IActionResult Visit(Im1ConnectionRegisterResult.SupplierSystemUnavailable result)
            {
                return new StatusCodeResult(StatusCodes.Status502BadGateway);
            }
        }
    }
}