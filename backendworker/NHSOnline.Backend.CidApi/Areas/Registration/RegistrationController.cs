using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.CidApi.Areas.Registration.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.CidApi.Areas.Registration
{
    [ApiVersion("2")]
    public class RegistrationController : Controller
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly IOdsCodeMassager _odsCodeMassager;
        private readonly IOdsCodeLookup _odsCodeLookup;

        public RegistrationController(
            ILogger<RegistrationController> logger,
            IOdsCodeMassager odsCodeMassager,
            IOdsCodeLookup odsCodeLookup)
        {
            _logger = logger;
            _odsCodeMassager = odsCodeMassager;
            _odsCodeLookup = odsCodeLookup;
        }

        [HttpGet, AllowAnonymous]
        [ApiVersionRoute("gp-practice/{odsCode}")]
        public async Task<IActionResult> Get([FromRoute(Name = "odsCode")] string odsCode)
        {

            try
            {
                _logger.LogEnter();

                if (odsCode != null)
                {
                    odsCode = _odsCodeMassager.CheckOdsCode(odsCode);
                }

                var supplier = await _odsCodeLookup.LookupSupplier(odsCode);
                GpSystemSupplier gpSystem;

                if (supplier.IsEmpty)
                {
                    _logger.LogInformation($"GP supplier for ODS Code: {odsCode} not found.");
                    return GpSystemNotFound(odsCode);
                }

                switch (supplier.ValueOrFailure())
                {
                    case Supplier.Emis:
                        gpSystem =  GpSystemSupplier.EMIS;
                        break;
                    case Supplier.Microtest:
                        gpSystem = GpSystemSupplier.Microtest;
                        break;
                    case Supplier.Tpp:
                        gpSystem =  GpSystemSupplier.TPP;
                        break;
                    case Supplier.Vision:
                        gpSystem = GpSystemSupplier.Vision;
                        break;
                    case Supplier.Fake:
                        gpSystem = GpSystemSupplier.NhsTestGpSupplier;
                        break;
                    default:
                        return GpSystemNotFound(odsCode);
                }

                return new OkObjectResult(
                        new GpSystemSupplierResponse
                        {
                            OdsCode = odsCode,
                            GpSystemSupplier = Enum.GetName(typeof(GpSystemSupplier), gpSystem),
                        }
                    );
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception occured, returning 500: {e}");
                return new ObjectResult(
                    new GpSystemSupplierErrorResponse { ErrorReason = e.Message, OdsCode = odsCode})
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private NotFoundObjectResult GpSystemNotFound(string odsCode)
        {
            return new NotFoundObjectResult(new GpSystemSupplierErrorResponse
            {
                OdsCode = odsCode,
                ErrorReason = $"GP supplier for ODS Code: {odsCode} not found."
            });
        }
    }
}