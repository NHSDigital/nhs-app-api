using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections;
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
            try
            {
                _logger.LogEnter();

                var parseSuccess = Enum.TryParse<VisionRecordSectionType>(section, true, out var visionSection);
                if (!parseSuccess)
                {
                    return BadRequest($"The section '{section}' is not valid");
                }

                var gpSystem = userSession.GpUserSession.Supplier;
                if (gpSystem != Supplier.Vision)
                {
                    return BadRequest("The Section endpoint only works with Vision");
                }

                await _auditor.Audit(AuditingOperations.ViewPatientRecordSectionAuditTypeRequest,
                    $"Viewing Patient Record {section}");

                var patientRecordService = GetVisionRecordService();
                if (patientRecordService is null)
                {
                    return BadRequest("The Section endpoint only works with Vision");
                }

                _logger.LogInformation($"Fetching patient record {section} section");
                var result = await patientRecordService.GetSection(userSession.GpUserSession, visionSection);

                await result.Accept(new MyRecordSectionAuditingVisitor(_auditor, _logger));
                return result.Accept(new MyRecordSectionResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to get section");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private IVisionPatientRecordService GetVisionRecordService()
        {
            _logger.LogInformation(
                $"Fetching PatientRecordService for Vision");
            return _gpSystemFactory
                .CreateGpSystem(Supplier.Vision)
                .GetPatientRecordService() as IVisionPatientRecordService;

        }
    }
}