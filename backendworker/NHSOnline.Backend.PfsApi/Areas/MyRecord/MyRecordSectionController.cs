using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    [ApiVersionRoute("patient/my-record/section")]
    public class MyRecordSectionController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<MyRecordSectionController> _logger;
        private readonly IAuditor _auditor;

        private readonly IEnumerable<VisionMapperType> _validSections = new[]
        {
            VisionMapperType.TestResults,
            VisionMapperType.Diagnosis,
            VisionMapperType.Examinations,
            VisionMapperType.Procedures
        };


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
        public async Task<IActionResult> GetSection(
            [FromQuery] string section,
            [UserSession] P9UserSession userSession)
        {
            _logger.LogEnter();
            var validVisionSection = GetVisionSection(section);
            if (validVisionSection is null)
            {
                return new BadRequestObjectResult(ModelState);
            }

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

            var result = await patientRecordService.GetSection(userSession.GpUserSession, validVisionSection.Value);

            // Audit result of attempt to view patient record    
            await result.Accept(new MyRecordSectionAuditingVisitor(_auditor, _logger));

            _logger.LogExit();
            return result.Accept(new MyRecordSectionResultVisitor());
        }

        private VisionMapperType? GetVisionSection(string section)
        {
            var sectionParseSuccess = Enum.TryParse<VisionMapperType>(section, out var visionSection);
            if (!sectionParseSuccess)
            {
                _logger.LogError($"Requested Vision My Record Section Invalid: '{section}'");
                return null;
            }
            if (!_validSections.Contains(visionSection))
            {
                _logger.LogError($"Requested Vision My Record Section Invalid: '{section}'");
                return null;
            }
            _logger.LogError($"Requested Vision My Record Section Valid: '{section}'");
            return visionSection;
        }
    }
}