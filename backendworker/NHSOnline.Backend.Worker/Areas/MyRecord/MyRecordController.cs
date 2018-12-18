using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    [Route("patient/my-record"),PfsSecurityMode]
    public class MyRecordController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<MyRecordController> _logger;
        private readonly IAuditor _auditor;
        
        public MyRecordController(
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory, 
            IAuditor auditor)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = loggerFactory.CreateLogger<MyRecordController>();
            _auditor = auditor;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyRecord()
        {   
            _logger.LogEnter();
            
            var userSession = HttpContext.GetUserSession();
            
            // Audit attempt made to view patient record
            await _auditor.Audit(Constants.AuditingTitles.ViewPatientRecordAuditTypeRequest, "Viewing Patient Record");
 
            _logger.LogInformation($"Fetching PatientRecordService for supplier: {userSession.GpUserSession.Supplier}");           
            var patientRecordService = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetPatientRecordService();

            _logger.LogInformation("Fetching patient record");
            var result = await patientRecordService.GetMyRecord(userSession);
            
            // Audit result of attempt to view patient record    
            result.Accept(new MyRecordAuditingVisitor(_auditor));
            
            _logger.LogExit();
            return result.Accept(new MyRecordResultVisitor());
        }
    }
}