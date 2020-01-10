using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    [ApiVersionRoute("patient/my-record")]
    public class MyRecordController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<MyRecordController> _logger;
        private readonly IAuditor _auditor;
        private readonly IMyRecordMetadataLogger _myRecordMetadataLogger;
        
        public MyRecordController(
            ILogger<MyRecordController> logger,
            IGpSystemFactory gpSystemFactory, 
            IAuditor auditor,
            IMyRecordMetadataLogger myRecordMetadataLogger)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
            _auditor = auditor;
            _myRecordMetadataLogger = myRecordMetadataLogger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyRecord([FromHeader(Name=PatientId)] Guid patientId)
        {   
            _logger.LogEnter();
            
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            
            _logger.LogDebug($"{nameof(GetMyRecord)} with patientId {patientId}");
   
            var userSession = HttpContext.GetUserSession();

            // Audit attempt made to view patient record
            await _auditor.Audit(AuditingOperations.ViewPatientRecordAuditTypeRequest, "Viewing Patient Record");
 
            _logger.LogInformation($"Fetching PatientRecordService for supplier: {userSession.GpUserSession.Supplier}");           
            var patientRecordService = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetPatientRecordService();

            var gpLinkedAccountUserSession = new GpLinkedAccountModel(
                userSession.GpUserSession, patientId
            );
            _logger.LogInformation("Fetching patient record");
            var result = await patientRecordService.GetMyRecord(gpLinkedAccountUserSession);
            
            // Audit result of attempt to view patient record
            await result.Accept(new MyRecordAuditingVisitor(_auditor, _logger));

            LogMetadata(userSession, result);
            
            _logger.LogExit();
            return result.Accept(new MyRecordResultVisitor());
        }

        private void LogMetadata(UserSession userSession, GetMyRecordResult result)
        {
            try
            {
                _myRecordMetadataLogger.LogMyRecordMetadata(userSession, result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to log Patient Record Metadata. " +
                                    "Catching exception to prevent inability to view medical record.");
            }
        }
    }
}
