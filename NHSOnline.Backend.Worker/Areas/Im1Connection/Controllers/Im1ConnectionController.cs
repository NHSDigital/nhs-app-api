using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
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

            var im1ConnectionService = await GetIm1ConnectionService(odsCode);

            if (!im1ConnectionService.HasValue)
            {
                return NotFound();
            }

            var verifyResult = await im1ConnectionService.ValueOrFailure().Verify(connectionToken, odsCode);

            return verifyResult.Accept(new Im1ConnectionVerifyResultVisitor());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PatientIm1ConnectionRequest model)
        {
            var im1ConnectionService = await GetIm1ConnectionService(model.OdsCode);

            if (!im1ConnectionService.HasValue)
            {
                return NotFound();
            }

            var registerResult = await im1ConnectionService.ValueOrFailure().Register(model);

            return registerResult.Accept(new Im1ConnectionRegisterResultVisitor(Request));
        }

        private async Task<Option<IIm1ConnectionService>> GetIm1ConnectionService(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplierAsync(odsCode);
            if (!supplier.HasValue)
            {
                return Option.None<IIm1ConnectionService>();
            }

            ISystemProvider systemProvider;

            try
            {
                systemProvider = _systemProviderFactory.CreateSystemProvider(supplier.ValueOrFailure());
            }
            catch (UnknownSupplierException)
            {
                return Option.None<IIm1ConnectionService>();
            }

            return Option.Some(systemProvider.GetIm1ConnectionService());
        }
    }
}
