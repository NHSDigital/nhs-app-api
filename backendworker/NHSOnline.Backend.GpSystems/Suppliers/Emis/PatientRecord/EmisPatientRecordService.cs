using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using HtmlToOpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using Wkhtmltopdf.NetCore;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisPatientRecordService : IPatientRecordService
    {
        private readonly ILogger<EmisPatientRecordService> _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisMyRecordMapper _emisMyRecordMapper;

        private readonly GetAllergiesTaskChecker _allergiesTaskChecker;
        private readonly GetMedicationsTaskChecker _medicationsTaskChecker;
        private readonly GetImmunisationsTaskChecker _immunistationsTaskChecker;
        private readonly GetTestResultsTaskChecker _testResultsTaskChecker;
        private readonly GetProblemsTaskChecker _problemsTaskChecker;
        private readonly GetConsultationsTaskChecker _consultationsTaskChecker;
        private readonly GetDocumentsTaskChecker _documentsTaskChecker;
        private readonly GetPatientDocumentTaskChecker _patientDocumentTaskChecker;

        private readonly IGeneratePdf _generatePdf;

        public EmisPatientRecordService(
            ILogger<EmisPatientRecordService> logger,
            IEmisClient emisClient, IEmisMyRecordMapper emisMyRecordMapper,
            GetAllergiesTaskChecker allergiesTaskChecker,
            GetMedicationsTaskChecker medicationsTaskChecker, 
            GetImmunisationsTaskChecker immunisationsTaskChecker,
            GetTestResultsTaskChecker testResultsTaskChecker, 
            GetProblemsTaskChecker problemsTaskChecker,
            GetConsultationsTaskChecker consultationsTaskChecker,
            GetDocumentsTaskChecker documentsTaskChecker,
            GetPatientDocumentTaskChecker patientDocumentTaskChecker,
            IGeneratePdf generatePdf
        )
        {
            _emisClient = emisClient;
            _emisMyRecordMapper = emisMyRecordMapper;
            _logger = logger;

            _allergiesTaskChecker =allergiesTaskChecker;
            _medicationsTaskChecker = medicationsTaskChecker;
            _immunistationsTaskChecker = immunisationsTaskChecker;
            _testResultsTaskChecker = testResultsTaskChecker;
            _problemsTaskChecker = problemsTaskChecker;
            _consultationsTaskChecker = consultationsTaskChecker;
            _documentsTaskChecker = documentsTaskChecker;
            _patientDocumentTaskChecker = patientDocumentTaskChecker;
            _generatePdf = generatePdf;
        }

        public async Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            var emisUserSession = (EmisUserSession)gpLinkedAccountModel.GpUserSession;

            try
            {
                EmisRequestParameters emisRequestParameters = gpLinkedAccountModel.BuildEmisRequestParameters(_logger);
                
                _logger.LogInformation("Creating patient record api tasks");
                var medicationsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Medication);

                var allergiesTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Allergies);

                var immunisationsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Immunisations);

                var testResultsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.TestResults);

                var problemsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Problems);

                var consultationsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Consultations);

                var documentsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Documents);

                await Task.WhenAll(allergiesTask, medicationsTask, immunisationsTask, testResultsTask, problemsTask, consultationsTask, documentsTask);
                _logger.LogInformation("Patient record tasks completed");

                _logger.LogInformation("Checking status of all patient record tasks");
                var allergies = _allergiesTaskChecker.Check(allergiesTask);
                var medications = _medicationsTaskChecker.Check(medicationsTask);
                var immunisations = _immunistationsTaskChecker.Check(immunisationsTask);
                var testResults = _testResultsTaskChecker.Check(testResultsTask);
                var problems = _problemsTaskChecker.Check(problemsTask);
                var consultations = _consultationsTaskChecker.Check(consultationsTask);
                var documents = _documentsTaskChecker.Check(documentsTask);

                _logger.LogInformation("Mapping EMIS responses to universal MyRecordResponse class instance");
                var myRecordResponse = _emisMyRecordMapper.Map(allergies, medications, immunisations, testResults, problems, consultations, documents);
                
                _logger.LogInformation("Number of documents to be returned " + myRecordResponse.Documents.RecordCount);
                myRecordResponse.Supplier = emisUserSession.Supplier.ToString().ToUpper(CultureInfo.InvariantCulture);

                return new GetMyRecordResult.Success(myRecordResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving my record");
                return new GetMyRecordResult.BadGateway();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "My record retrieval return null body");
                return new GetMyRecordResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetPatientDocumentResult> GetPatientDocument(GpUserSession gpUserSession, string documentGuid,
            string documentType, string documentName)
        {
            _logger.LogEnter();

            var emisUserSession = (EmisUserSession)gpUserSession;

            try
            {
                var getDocumentsTask = _emisClient.MedicalDocumentGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, documentGuid, emisUserSession.EndUserSessionId);

                await Task.WhenAll(getDocumentsTask);

                var documentResponse = _patientDocumentTaskChecker.Check(getDocumentsTask, documentType, documentName);

                if (documentResponse.HasErrored)
                {
                    _logger.LogExitWith($"{nameof(documentResponse.HasErrored)}=true");
                    return new GetPatientDocumentResult.BadGateway();
                }

                return new GetPatientDocumentResult.Success(documentResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving document");
                return new GetPatientDocumentResult.BadGateway();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "Record document retrieval return null body");
                return new GetPatientDocumentResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        public async Task<PatientDocument> GetPatientDocumentForDownload(GpUserSession gpUserSession, string documentGuid,
            string documentType, string documentName)
        {
            _logger.LogEnter();

            var emisUserSession = (EmisUserSession)gpUserSession;

            try
            {
                var getDocumentsTask = _emisClient.MedicalDocumentGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, documentGuid, emisUserSession.EndUserSessionId);

                await Task.WhenAll(getDocumentsTask);

                var documentResponse =  _patientDocumentTaskChecker.Check(getDocumentsTask, documentType, documentName);

                if (documentResponse.HasErrored)
                {
                    _logger.LogExitWith($"{nameof(documentResponse.HasErrored)}=true");
                    return documentResponse;
                }

                return documentResponse;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving document");
                return new PatientDocument {
                    HasErrored = true
                };
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "Record document retrieval return null body"); 
                return new PatientDocument {
                    HasErrored = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception when retrieving document for download"); 
                return new PatientDocument {
                    HasErrored = true
                };
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public Task<GetDetailedTestResult> GetDetailedTestResult(GpUserSession gpUserSession, string testResultId)
        {
            throw new NotImplementedException();
        }

        public byte[] ConvertDocumentToCorrectFormat(string type, string content)
        {
            var isImageType = Constants.FileConstants.FileTypes.ImageTypes.Contains(type);
            var isTextType = Constants.FileConstants.FileTypes.TextTypes.Contains(type);
            var isPdfType = type.Equals(Constants.FileConstants.FileTypes.DocumentType.Pdf, StringComparison.Ordinal);
            byte[] data = null;
            
            var doc = new HtmlDocument
            {
                OptionWriteEmptyNodes = true
            };

            doc.LoadHtml(content);
            

            if (isImageType || isTextType)
            {
                
                if (isImageType)
                {
                    data = IsImage(doc);
                }
                else
                {
                    _logger.LogInformation("File is a text type");
                    data = Encoding.UTF8.GetBytes(System.Net.WebUtility.HtmlDecode(doc.DocumentNode.InnerText)); 
                }
            }
            else if (Constants.FileConstants.FileTypes.DocumentTypes.Contains(type))
            {
                data = IsDocumentType(content);
            }
            else if (isPdfType)
            {
                _logger.LogInformation("File is a pdf type");
                var htmlContent = "<html><body>";
                var imgNodes = doc.DocumentNode.SelectNodes(".//img");

                // Make the image 100% width of the PDF page
                foreach (var imageNode in imgNodes)
                {
                    imageNode.Attributes.Add("width", "100%");
                }

                htmlContent += doc.DocumentNode.InnerHtml;
                htmlContent += "</body></html>";
                data = _generatePdf.GetPDF(htmlContent);
            }

            return data;
        }

         private byte[] IsImage(HtmlDocument document)
        {
            _logger.LogInformation("File is an image type");
            var imgNodes = document.DocumentNode.SelectNodes(".//img");
            
            byte[] data;

            if (imgNodes == null)
            {
                _logger.LogInformation("Document contains no img tag. Returning null data");
                return null;
            }

            data = Convert.FromBase64String(imgNodes[0].GetAttributeValue("src", "").Split("base64,")[1]);

            return data;
        }

        private byte[] IsDocumentType(string content)
        {
            _logger.LogInformation("File is a document type");
            using (var generatedDocument = new MemoryStream())
            {
                using (var package =
                    WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
                {
                    var mainPart = package.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = package.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }

                    var converter = new HtmlConverter(mainPart);
                    converter.ParseHtml(content);

                    mainPart.Document.Save();
                }

                return generatedDocument.ToArray();
            }
        }
    }
}