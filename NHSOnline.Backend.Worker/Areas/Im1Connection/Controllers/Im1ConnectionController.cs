using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Ods;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Im1Connection;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection.Controllers
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

        [HttpGet, TimeoutExceptionFilter]
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

            var systemProviderOption = await GetSystemProvider(odsCode);
            if (!systemProviderOption.HasValue)
            {
                // If no system provider is returned it is because the supplier could not be determined from the ODS
                // code.  This, in turn, is because the ODS code is not found so a "Not Found" response is returned.
                return NotFound();
            }

            var systemProvider = systemProviderOption.Value;
            var tokenValidationService = systemProvider.GetTokenValidationService();
            if (!tokenValidationService.IsValidConnectionTokenFormat(connectionToken))
            {
                return BadRequest();
            }

            var im1ConnectionService = systemProvider.GetIm1ConnectionService();
            var verifyResult = await im1ConnectionService.VerifyAsync(connectionToken, odsCode);

            return verifyResult.Accept(new Im1ConnectionVerifyResultVisitor());
        }

        [HttpPost, TimeoutExceptionFilter]
        public async Task<IActionResult> Post([FromBody] PatientIm1ConnectionRequest model)
        {
            var systemProviderOption = await GetSystemProvider(model.OdsCode);
            if (!systemProviderOption.HasValue)
            {
                return NotFound();
            }

            var systemProvider = systemProviderOption.Value;
            var im1ConnectionService = systemProvider.GetIm1ConnectionService();
            var registerResult = await im1ConnectionService.RegisterAsync(model);

            return registerResult.Accept(new Im1ConnectionRegisterResultVisitor(Request));
        }

        private async Task<Option<ISystemProvider>> GetSystemProvider(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplierAsync(odsCode);
            if (!supplier.HasValue)
            {
                return Option.None<ISystemProvider>();
            }

            return Option.Some(_systemProviderFactory.CreateSystemProvider(supplier.ValueOrFailure()));
        }
    }
}
