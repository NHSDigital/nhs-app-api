using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    [Route("patient/my-record/section")]
    public class MyRecordSectionController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<MyRecordSectionController> _logger;
        private readonly IAuditor _auditor;

        public MyRecordSectionController(
            ILogger<MyRecordSectionController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpGet]
        public async Task<IActionResult> GetSection([FromQuery] VisionMapperType section)
        {
            _logger.LogEnter();
            
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var userSession = HttpContext.GetUserSession();

            var gpSystem = userSession.GpUserSession.Supplier;

            if (gpSystem != Supplier.Vision)
            {
                return BadRequest("The Test Results endpoint only works with Vision");
            }

            await _auditor.Audit(AuditingOperations.ViewPatientRecordSectionAuditTypeRequest,
                $"Viewing Patient Record {section}");

            _logger.LogInformation($"Fetching PatientRecordService for supplier: {userSession.GpUserSession.Supplier}");

            var patientRecordService = _gpSystemFactory
                .CreateGpSystem(gpSystem)
                .GetPatientRecordService() as IVisionPatientRecordService;

            _logger.LogInformation($"Fetching patient record {section} section");

            var result = await patientRecordService.GetSection(userSession.GpUserSession, section);

            // Audit result of attempt to view patient record    
            await result.Accept(new MyRecordSectionAuditingVisitor(_auditor, _logger));

            _logger.LogExit();
            return result.Accept(new MyRecordSectionResultVisitor());
        }
    }
}