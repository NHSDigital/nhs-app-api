using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientMessagesController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<PatientMessagesController> _logger;
        private readonly IAuditor _auditor;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        public PatientMessagesController(
            IGpSystemFactory gpSystemFactory,
            ILogger<PatientMessagesController> logger,
            IAuditor auditor,
            IErrorReferenceGenerator errorReferenceGenerator)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
            _auditor = auditor;
            _errorReferenceGenerator = errorReferenceGenerator;
        }
        
        [HttpGet]
        [Route("patient/messages")]
        public async Task<IActionResult> GetMessages()
        {
            _logger.LogEnter();
            
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            
            await _auditor.Audit(AuditingOperations.ViewPracticePatientMessagesRequest, "Viewing Practice to Patient Messages");

            var userSession = HttpContext.GetUserSession();
            var gpUserSession = userSession.GpUserSession;
            
            _logger.LogInformation($"Fetching PatientMessagesService for supplier: {gpUserSession.Supplier}");
            
            var patientMessagesService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetPatientMessagesService();

            var result = await patientMessagesService.GetMessages(gpUserSession);
            
            await result.Accept(new PatientMessagesResultAuditingVisitor(_auditor, _logger));
            return result.Accept(new PatientMessagesResultVisitor(_errorReferenceGenerator, userSession));
        }
        
        [HttpGet]
        [Route("patient/messages/{messageId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMessageDetails(
            [FromRoute(Name = "messageId")] string messageId)
        {
            _logger.LogEnter();
            
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var userSession = HttpContext.GetUserSession();
            var gpUserSession = userSession.GpUserSession;
            
            _logger.LogInformation($"Fetching PatientMessagesService for supplier: {gpUserSession.Supplier}");
            
            var patientMessagesService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetPatientMessagesService();

            var result = await patientMessagesService.GetMessageDetails(messageId, gpUserSession);
            
            return result.Accept(new PatientMessageResultVisitor(_errorReferenceGenerator, userSession));
        }
    }
}