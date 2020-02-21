using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using HtmlToOpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using Wkhtmltopdf.NetCore;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    internal sealed class EmisPatientRecordService : IPatientRecordService
    {
        private readonly ILogger<EmisPatientRecordService> _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisMyRecordMapper _emisMyRecordMapper;

        private readonly GetAllergiesTaskChecker _allergiesTaskChecker;
        private readonly GetMedicationsTaskChecker _medicationsTaskChecker;
        private readonly GetImmunisationsTaskChecker _immunisationsTaskChecker;
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
            _immunisationsTaskChecker = immunisationsTaskChecker;
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
                var emisRequestParameters = gpLinkedAccountModel.BuildEmisRequestParameters(_logger);

                _logger.LogInformation("Creating patient record api tasks");
                var allergiesTask = RetrieveAllergies(emisRequestParameters);
                var medicationsTask = RetrieveMedications(emisRequestParameters);
                var immunisationsTask = RetrieveImmunisations(emisRequestParameters);
                var testResultsTask = RetrieveTestResults(emisRequestParameters);
                var problemsTask = RetrieveProblems(emisRequestParameters);
                var consultationsTask = RetrieveConsultations(emisRequestParameters);
                var documentsTask = RetrieveDocuments(emisRequestParameters);

                await Task.WhenAll(allergiesTask, medicationsTask, immunisationsTask, testResultsTask, problemsTask, consultationsTask, documentsTask);

                _logger.LogInformation("Patient record tasks completed");

                _logger.LogInformation("Mapping EMIS responses to universal MyRecordResponse class instance");
                var myRecordResponse = _emisMyRecordMapper.Map(allergiesTask.Result, medicationsTask.Result, immunisationsTask.Result,
                    testResultsTask.Result, problemsTask.Result, consultationsTask.Result, documentsTask.Result);

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

        public async Task<GetPatientDocumentResult> GetPatientDocument(GpUserSession gpUserSession, string documentIdentifier,
            string documentType, string documentName)
        {
            _logger.LogEnter();

            var emisUserSession = (EmisUserSession)gpUserSession;

            try
            {
                var getDocumentsTask = _emisClient.MedicalDocumentGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, documentIdentifier, emisUserSession.EndUserSessionId);

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

        public async Task<PatientDocument> GetPatientDocumentForDownload(GpUserSession gpUserSession, string documentIdentifier,
            string documentType, string documentName)
        {
            _logger.LogEnter();

            var emisUserSession = (EmisUserSession)gpUserSession;

            try
            {
                var getDocumentsTask = _emisClient.MedicalDocumentGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, documentIdentifier, emisUserSession.EndUserSessionId);

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

        private async Task<Allergies> RetrieveAllergies(EmisRequestParameters emisRequestParameters)
        {
            _logger.LogInformation("Retrieving allergies.");
            try
            {
                var allergiesTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Allergies);
                await allergiesTask;
                return _allergiesTaskChecker.Check(allergiesTask);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Retrieving allergies failed. Returning hasErrored as true");
                return new Allergies
                {
                    HasErrored = true
                };
            }
        }
        private async Task<Medications> RetrieveMedications(EmisRequestParameters emisRequestParameters)
        {
            _logger.LogInformation("Retrieving medications.");
            try
            {
                var medicationsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Medication);
                await medicationsTask;
                return _medicationsTaskChecker.Check(medicationsTask);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Retrieving medications failed. Returning hasErrored as true");
                return new Medications
                {
                    HasErrored = true
                };
            }
        }
        private async Task<Immunisations> RetrieveImmunisations(EmisRequestParameters emisRequestParameters)
        {
            _logger.LogInformation("Retrieving immunisations.");
            try
            {
                var immunisationsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Immunisations);
                await immunisationsTask;
                return _immunisationsTaskChecker.Check(immunisationsTask);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Retrieving immunisations failed. Returning hasErrored as true");
                return new Immunisations
                {
                    HasErrored = true
                };
            }
        }
        private async Task<TestResults> RetrieveTestResults(EmisRequestParameters emisRequestParameters)
        {
            _logger.LogInformation("Retrieving test results.");
            try
            {
                var testResultsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.TestResults);
                await testResultsTask;
                return _testResultsTaskChecker.Check(testResultsTask);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Retrieving test results failed. Returning hasErrored as true");
                return new TestResults
                {
                    HasErrored = true
                };
            }
        }
        private async Task<Problems> RetrieveProblems(EmisRequestParameters emisRequestParameters)
        {
            _logger.LogInformation("Retrieving problems.");
            try
            {
                var problemsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Problems);
                await problemsTask;
                return _problemsTaskChecker.Check(problemsTask);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Retrieving consultations failed. Returning hasErrored as true");
                return new Problems
                {
                    HasErrored = true
                };
            }
        }
        private async Task<Consultations> RetrieveConsultations(EmisRequestParameters emisRequestParameters)
        {
            _logger.LogInformation("Retrieving consultations.");
            try
            {
                var consultationsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Consultations);
                await consultationsTask;
                return _consultationsTaskChecker.Check(consultationsTask);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Retrieving consultations failed. Returning hasErrored as true");
                return new Consultations
                {
                    HasErrored = true
                };
            }
        }
        private async Task<PatientDocuments> RetrieveDocuments(EmisRequestParameters emisRequestParameters)
        {
            _logger.LogInformation("Retrieving documents.");
            try
            {
                var documentsTask = _emisClient.MedicalRecordGet(emisRequestParameters, RecordType.Documents);
                await documentsTask;
                return _documentsTaskChecker.Check(documentsTask);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Retrieving documents failed. Returning hasErrored as true");
                return new PatientDocuments
                {
                    HasErrored = true
                };
            }
        }

    }
}