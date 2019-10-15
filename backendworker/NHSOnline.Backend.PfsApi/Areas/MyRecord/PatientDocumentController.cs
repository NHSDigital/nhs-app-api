using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    
    public class PatientDocumentController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<PatientDocumentController> _logger;
        private readonly IAuditor _auditor;
        
        public PatientDocumentController(
            ILogger<PatientDocumentController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpPost]
        [Route("documents/{documentGuid}")]
        public async Task<IActionResult> GetPatientDocument(
            [FromRoute(Name = "documentGuid")] string documentGuid,
            [FromBody] DocumentInfo documentInfo)
        {
            try
            {
                _logger.LogEnter();
                
                await _auditor.Audit(AuditingOperations.GetDocumentAuditTypeRequest,
                    "Attempting to view document");

                var userSession = HttpContext.GetUserSession();

                _logger.LogInformation($"Fetching PatientRecordService for supplier: {userSession.GpUserSession}");
                var patientRecordService = _gpSystemFactory
                    .CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetPatientRecordService();

                _logger.LogInformation("Fetching patient document");
                var result = await patientRecordService.GetPatientDocument(
                    userSession.GpUserSession,
                    documentGuid,
                    documentInfo.Type,
                    documentInfo.Name);
                
                await result.Accept(new PatientDocumentAuditingVisitor(_auditor, _logger));
                return result.Accept(new PatientDocumentVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}