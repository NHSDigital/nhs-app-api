using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using FileTypes = NHSOnline.Backend.Support.Constants.FileConstants.FileTypes;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

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
        [ApiVersionRoute("documents/{documentIdentifier}")]
        public async Task<IActionResult> GetPatientDocument(
            [FromHeader(Name=PatientId)] Guid patientId,
            [FromRoute(Name="documentIdentifier")] string documentIdentifier,
            [FromBody] DocumentInfo documentInfo)
        {
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                await _auditor.Audit(AuditingOperations.ViewDocumentAuditTypeRequest,
                    "Viewing patient document");

                var userSession = HttpContext.GetUserSession();

                _logger.LogInformation($"Fetching PatientRecordService for supplier: {userSession.GpUserSession}");
                var patientRecordService = _gpSystemFactory
                    .CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetPatientRecordService();

                var gpLinkedAccountModel = new GpLinkedAccountModel(
                    userSession.GpUserSession, patientId
                );

                _logger.LogInformation("Fetching patient document");
                var result = await patientRecordService.GetPatientDocument(
                    gpLinkedAccountModel,
                    documentIdentifier,
                    documentInfo.Type,
                    documentInfo.Name);

                await result.Accept(new PatientDocumentAuditingVisitor(_auditor, _logger));
                return result.Accept(new PatientDocumentResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        [ApiVersionRoute("documents/{documentIdentifier}/download")]
        public async Task<IActionResult> GetPatientDocumentForDownload(
            [FromHeader(Name=PatientId)] Guid patientId,
            [FromRoute(Name = "documentIdentifier")] string documentIdentifier,
            [FromBody] DocumentInfo documentInfo)
        {
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                await _auditor.Audit(AuditingOperations.DownloadDocumentAuditTypeRequest,
                    "Downloading patient document");

                var userSession = HttpContext.GetUserSession();
                var gpLinkedAccountModel = new GpLinkedAccountModel(
                    userSession.GpUserSession, patientId
                );

                _logger.LogInformation("Fetching PatientRecordService for supplier");

                var patientRecordService = _gpSystemFactory
                    .CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetPatientRecordService();

                _logger.LogInformation("Fetching patient document for download");

                var result = await patientRecordService.GetPatientDocumentForDownload(
                    gpLinkedAccountModel,
                    documentIdentifier,
                    documentInfo.Type,
                    documentInfo.Name);

                if (result.GetType() == typeof(GetPatientDocumentDownloadResult.Success))
                {
                    var response = ((GetPatientDocumentDownloadResult.Success) result).Response;
                    Response.ContentType = response.ContentType;
                    Response.Headers.ContentLength = response.FileContents.LongLength;
                    Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{response.FileDownloadName}\"");
                }

                await result.Accept(new PatientDocumentDownloadAuditingVisitor(_auditor, _logger));
                return result.Accept(new PatientDocumentDownloadResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}