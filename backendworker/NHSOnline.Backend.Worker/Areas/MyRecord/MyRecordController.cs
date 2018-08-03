using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    [Route("patient/my-record")]
    public class MyRecordController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger _logger;
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
        [TimeoutExceptionFilter]
        public async Task<IActionResult> GetMyRecord()
        {   
            var methodName = "GetMyRecord";
            _logger.LogDebug("Entered: {0}", methodName);
            
            var userSession = HttpContext.GetUserSession();
            
            // Audit attempt made to view patient record
            _auditor.Audit(Constants.AuditingTitles.ViewPatientRecordAuditTypeRequest, "Viewing Patient Record");
 
            _logger.LogInformation("Fetching PatientRecordService for supplier: {0}", userSession.Supplier.ToString());           
            var patientRecordService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetPatientRecordService();

            _logger.LogInformation("Fetching patient record");
            var result = await patientRecordService.Get(userSession);
            
            // Audit result of attempt to view patient record    
            result.Accept(new MyRecordAuditingVisitor(_auditor));
            
            _logger.LogDebug("Exiting: {0}", methodName);
            return result.Accept(new MyRecordResultVisitor());
        }
    }
}