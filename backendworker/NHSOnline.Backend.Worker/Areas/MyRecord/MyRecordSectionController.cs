using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    [Route("patient/my-record/section"), PfsSecurityMode]
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

            var userSession = HttpContext.GetUserSession();

            var gpSystem = userSession.GpUserSession.Supplier;

            if (gpSystem != Supplier.Vision)
            {
                return BadRequest("The Test Results endpoint only works with Vision");
            }

            await _auditor.Audit(Constants.AuditingTitles.ViewPatientRecordSectionAuditTypeRequest,
                $"Viewing Patient Record {section}");

            _logger.LogInformation($"Fetching PatientRecordService for supplier: {userSession.GpUserSession.Supplier}");

            var patientRecordService = _gpSystemFactory
                .CreateGpSystem(gpSystem)
                .GetPatientRecordService() as IVisionPatientRecordService;

            _logger.LogInformation($"Fetching patient record {section} section");

            var result = await patientRecordService.GetSection(userSession, section);

            // Audit result of attempt to view patient record    
            await result.Accept(new MyRecordSectionAuditingVisitor(_auditor, _logger));

            _logger.LogExit();
            return result.Accept(new MyRecordSectionResultVisitor());
        }
    }
}