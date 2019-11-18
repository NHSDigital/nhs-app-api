using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using FileTypes = NHSOnline.Backend.Support.Constants.FileConstants.FileTypes;

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
                
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }
                
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
        
        [HttpPost]
        [Route("documents/{documentGuid}/download")]
        public async Task<IActionResult> GetPatientDocumentForDownload(            
            [FromRoute(Name = "documentGuid")] string documentGuid,
            [FromBody] DocumentInfo documentInfo)
        {
            try
            {
                _logger.LogEnter();
                
                _logger.LogInformation("Fetching PatientRecordService for supplier");

                var userSession = HttpContext.GetUserSession();
                
                var patientRecordService = _gpSystemFactory
                    .CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetPatientRecordService();
                
                var type = documentInfo.Type;
                var name = documentInfo.Name;
                
                var result = await patientRecordService.GetPatientDocumentForDownload(
                    userSession.GpUserSession,
                    documentGuid, documentInfo.Type, documentInfo.Name);
                
                var data = patientRecordService.ConvertDocumentToCorrectFormat(type, result.Content);

                if (data != null)
                {
                    type = MapFileTypeToDownloadType(type);
                    var mimeType = FileTypes.DocumentMimeTypes[type];
                    Response.ContentType = mimeType;
                    Response.Headers.ContentLength = data.LongLength;
                    Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{name}.{type}\"");

                    return File(data, mimeType, $"{name}.{type}");
                }
                else
                {
                    _logger.LogInformation("File data was null after conversion. Returning BadRequest");
                    return BadRequest();
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private static string MapFileTypeToDownloadType(string fileType)
        {
            // this should mimic the function in web pages/document/_id#mapFileTypeToDownloadType
            switch (fileType)
            {
                case FileTypes.DocumentType.Docm:
                    return FileTypes.DocumentType.Doc;
                case FileTypes.TextType.Rtf:
                    return FileTypes.TextType.Txt;
                case FileTypes.ImageType.Jfif:
                    return FileTypes.ImageType.Jpg;
                default:
                    return fileType;
            }
        }
    }
}