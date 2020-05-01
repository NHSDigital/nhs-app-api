using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

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
        [ApiVersionRoute("patient/messages")]
        public async Task<IActionResult> GetMessages([UserSession] P9UserSession userSession)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _auditor.Audit(AuditingOperations.ViewPatientPracticeMessagesRequest, "Viewing Patient to Practice Messages");

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
        [ApiVersionRoute("patient/messages/{messageId}")]
        public async Task<IActionResult> GetMessageDetails(
            [FromRoute(Name = "messageId")] string messageId,
            [UserSession] P9UserSession userSession)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _auditor.Audit(AuditingOperations.GetPatientPracticeMessageDetailsRequest, "Getting Patient to Practice Message Details");

            var gpUserSession = userSession.GpUserSession;

            _logger.LogInformation($"Fetching PatientMessagesService for supplier: {gpUserSession.Supplier}");

            var patientMessagesService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetPatientMessagesService();

            var result = await patientMessagesService.GetMessageDetails(messageId, gpUserSession);

            await result.Accept(new PatientMessageResultAuditingVisitor(_auditor, _logger));
            return result.Accept(new PatientMessageResultVisitor(_errorReferenceGenerator, userSession));
        }

        [HttpPut]
        [ApiVersionRoute("patient/messages/updateReadStatus")]
        public async Task<IActionResult> PostUpdateMessageReadStatus(
            [FromBody] UpdateMessageReadStatusRequestBody updateMessageReadStatusRequest,
            [UserSession] P9UserSession userSession)
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
        [ApiVersionRoute("patient/messages/recipients")]
        public async Task<IActionResult> GetMessageRecipients([UserSession] P9UserSession userSession)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _auditor.Audit(AuditingOperations.GetPatientPracticeMessageRecipientsRequest, "Getting Patient to Practice Message Recipients");

            var gpUserSession = userSession.GpUserSession;

            _logger.LogInformation($"Fetching PatientMessagesService for supplier: {gpUserSession.Supplier}");

            var patientMessagesService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetPatientMessagesService();

            var result = await patientMessagesService.GetMessageRecipients(gpUserSession);

            await result.Accept(new PatientMessageRecipientsResultAuditingVisitor(_auditor, _logger));
            return result.Accept(new PatientMessageRecipientsResultVisitor(_errorReferenceGenerator, userSession));
        }

        [HttpPost]
        [ApiVersionRoute("patient/messages")]
        public async Task<IActionResult> SendMessage(
            [FromBody] CreatePatientMessage message,
            [UserSession] P9UserSession userSession)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _auditor.Audit(AuditingOperations.CreatePatientPracticeMessageRequest,
                "Creating a patient to practice message");

            var gpUserSession = userSession.GpUserSession;

            _logger.LogInformation($"Fetching PatientMessagesService for supplier: {gpUserSession.Supplier}");

            var patientMessagesService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetPatientMessagesService();

            var result = await patientMessagesService.SendMessage(gpUserSession, message);

            await result.Accept(new PatientSendMessageResultAuditingVisitor(_auditor, _logger));
            return result.Accept(new PatientSendMessageResultVisitor(_errorReferenceGenerator, userSession));
        }

        [HttpDelete]
        [ApiVersionRoute("patient/messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(
            [FromRoute(Name = "messageId")] string messageId,
            [UserSession] P9UserSession userSession)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _auditor.Audit(AuditingOperations.DeletePatientPracticeMessageRequest,
                $"Deleting a patient to practice message with id {messageId}");

            var gpUserSession = userSession.GpUserSession;

            _logger.LogInformation($"Fetching PatientMessagesService for supplier: {gpUserSession.Supplier}");

            var patientMessagesService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetPatientMessagesService();

            var result = await patientMessagesService.DeleteMessage(gpUserSession, messageId);

            await result.Accept(new PatientMessageDeleteResultAuditingVisitor(_auditor, _logger));
            return result.Accept(new PatientMessageDeleteResultVisitor(_errorReferenceGenerator, userSession));
        }
    }
}