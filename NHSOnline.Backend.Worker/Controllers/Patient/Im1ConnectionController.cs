using System.Threading.Tasks;
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
            _odsCodeLookup = odsCodeLookup;
            _systemProviderFactory = systemProviderFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromHeader(Name = Headers.ConnectionToken)] string connectionToken,
            [FromHeader(Name = Headers.OdsCode)] string odsCode
        )
        {
            if (string.IsNullOrEmpty(connectionToken) || string.IsNullOrEmpty(odsCode))
            {
                return BadRequest();
            }

            var supplier = await _odsCodeLookup.LookupSupplier(odsCode);
            var systemProvider = _systemProviderFactory.CreateSystemProvider(supplier);
            var nhsNumberProvider = systemProvider.GetNhsNumberProvider();
            var nhsNumbers = await nhsNumberProvider.GetNhsNumbersAsync(connectionToken, odsCode);

            return Json(new PatientIm1ConnectionResponse
            {
                NhsNumbers = nhsNumbers
            });
        }
    }
}
