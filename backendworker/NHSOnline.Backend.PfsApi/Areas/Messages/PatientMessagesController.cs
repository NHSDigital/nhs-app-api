using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
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
            
            await _auditor.Audit(AuditingOperations.ViewPatientPracticeMessagesRequest, "Viewing Patient to Practice Messages");

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
        public async Task<IActionResult> GetMessageDetails(
            [FromRoute(Name = "messageId")] string messageId)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _auditor.Audit(AuditingOperations.GetPatientPracticeMessageDetailsRequest, "Getting Patient to Practice Message Details");
            
            var userSession = HttpContext.GetUserSession();
            var gpUserSession = userSession.GpUserSession;

            _logger.LogInformation($"Fetching PatientMessagesService for supplier: {gpUserSession.Supplier}");

            var patientMessagesService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetPatientMessagesService();

            var result = await patientMessagesService.GetMessageDetails(messageId, gpUserSession);
            
            await result.Accept(new PatientMessageResultAuditingVisitor(_auditor, _logger));
            return result.Accept(new PatientMessageResultVisitor(_errorReferenceGenerator, userSession));
        }

        [HttpPost]
        [Route("patient/messages/updateReadStatus")]
        public async Task<IActionResult> PostUpdateMessageReadStatus([FromBody] UpdateMessageReadStatusRequestBody updateMessageReadStatusRequest)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            
            await _auditor.Audit(
                AuditingOperations.UpdatePatientPracticeMessageUnreadStatusRequest,
                $"Updating unread status for message with id {updateMessageReadStatusRequest.MessageId} to " +
                $"{updateMessageReadStatusRequest.MessageReadState}");

            var userSession = HttpContext.GetUserSession();
            var gpUserSession = userSession.GpUserSession;

            _logger.LogInformation($"Fetching PatientMessagesService for supplier: {gpUserSession.Supplier}");

            var patientMessagesService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetPatientMessagesService();

            var result = await patientMessagesService.UpdateMessageMessageReadStatus(gpUserSession, updateMessageReadStatusRequest);

            await result.Accept(new PatientMessageUpdateReadStatusAuditingVisitor(_auditor, _logger, updateMessageReadStatusRequest));
            return result.Accept(new PatientMessageUpdateReadStatusResultVisitor(_errorReferenceGenerator, userSession));
        }

        [HttpGet]
        [Route("patient/messages/recipients")]
        public async Task<IActionResult> GetMessageRecipients()
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _auditor.Audit(AuditingOperations.GetPatientPracticeMessageRecipientsRequest, "Getting Patient to Practice Message Recipients");

            var userSession = HttpContext.GetUserSession();
            var gpUserSession = userSession.GpUserSession;
            
            _logger.LogInformation($"Fetching PatientMessagesService for supplier: {gpUserSession.Supplier}");
            
            var patientMessagesService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetPatientMessagesService();

            var result = await patientMessagesService.GetMessageRecipients(gpUserSession);
            
            await result.Accept(new PatientMessageRecipientsResultAuditingVisitor(_auditor, _logger));
            return result.Accept(new PatientMessageRecipientsResultVisitor(_errorReferenceGenerator, userSession));
        }
    }
}