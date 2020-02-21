using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    internal sealed class TppPatientRecordService : IPatientRecordService
    {
        private const string TestResultDateFormat = "yyyy-MM-ddTHH:mm:ss.f'Z'";
        private readonly ILogger<TppPatientRecordService> _logger;
        private readonly IGetPatientDcrEventsTaskChecker _patientDcrEventsChecker;
        private readonly IGetPatientDocumentsFromDcrEventsTaskChecker _patientDocumentsFromDcrEventsTaskChecker;
        private readonly IGetPatientOverviewTaskChecker _patientOverviewTaskChecker;
        private readonly IGetPatientDocumentTaskChecker _patientDocumentTaskChecker;
        private readonly IGetPatientTestResultsTaskChecker _patientTestResultsChecker;
        private readonly IGetTppDetailedTestResultChecker _patientDetailedTestResultChecker;
        private readonly ITppClient _tppClient;
        private readonly ITppMyRecordMapper _tppMyRecordMapper;
        private readonly ITppClientRequest<TppUserSession, ViewPatientOverviewReply> _patientOverview;
        private readonly ITppClientRequest<TppUserSession, RequestPatientRecordReply> _requestPatientRecord;
        private readonly ITppClientRequest<(TppUserSession tppUserSession, string startDate, string endDate), TestResultsViewReply> _testResultsView;

        public TppPatientRecordService(IGetPatientDcrEventsTaskChecker patientDcrEventsChecker,
            IGetPatientOverviewTaskChecker patientOverviewTaskChecker,
            IGetPatientDocumentTaskChecker patientDocumentTaskChecker,
            IGetPatientTestResultsTaskChecker patientTestResultsChecker,
            IGetTppDetailedTestResultChecker patientDetailedTestResultChecker,
            ITppClientRequest<TppUserSession, ViewPatientOverviewReply> patientOverview,
            IGetPatientDocumentsFromDcrEventsTaskChecker patientDocumentsFromDcrEventsTaskChecker,
            ILogger<TppPatientRecordService> logger, ITppClient tppClient, ITppMyRecordMapper tppMyRecordMapper,
            ITppClientRequest<TppUserSession, RequestPatientRecordReply> requestPatientRecord,
            ITppClientRequest<(TppUserSession tppUserSession, string startDate, string endDate), TestResultsViewReply> testResultsView)
        {
            _patientDcrEventsChecker = patientDcrEventsChecker;
            _patientOverviewTaskChecker = patientOverviewTaskChecker;
            _patientDocumentTaskChecker = patientDocumentTaskChecker;
            _patientTestResultsChecker = patientTestResultsChecker;
            _patientDetailedTestResultChecker = patientDetailedTestResultChecker;
            _patientOverview = patientOverview;
            _patientDocumentsFromDcrEventsTaskChecker = patientDocumentsFromDcrEventsTaskChecker;
            _tppClient = tppClient;
            _tppMyRecordMapper = tppMyRecordMapper;
            _requestPatientRecord = requestPatientRecord;
            _testResultsView = testResultsView;
            _logger = logger;
        }

        public async Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            var tppUserSession = (TppUserSession)gpLinkedAccountModel.GpUserSession;

            try
            {
                // TPP does not yet handle concurrent requests
                var patientOverviewItems = await RetrievePatientOverviewItems(tppUserSession);
                var dcrItems = await RetrievePatientRecordDcrItems(tppUserSession);
                var documentItems = dcrItems.PatientDocuments;
                var dcrEvents = dcrItems.TppDcrEvents;
                var testResults = await RetrieveTestResults(tppUserSession);

                _logger.LogInformation($"Number of documents for user " +
                                       $"at ODSCode {tppUserSession.OdsCode} is " +
                                       $"{documentItems.Data.Count()}");

                var allergies = patientOverviewItems.Item1;
                var medications = patientOverviewItems.Item2;

                _logger.LogInformation($"Mapping TPP responses to universal {nameof(MyRecordResponse)} class instance");
                var myRecordResponse = _tppMyRecordMapper.Map(allergies, medications,
                    dcrEvents,
                    testResults,
                    documentItems);
                myRecordResponse.Supplier = tppUserSession.Supplier.ToString().ToUpper(CultureInfo.InvariantCulture);

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

            var tppUserSession = (TppUserSession) gpUserSession;

            try
            {
                var patientOverviewItems = await RetrievePatientDocument(documentIdentifier, tppUserSession);

                _logger.LogInformation($"Document type for user document " +
                                       $"at ODSCode {tppUserSession.OdsCode} is " +
                                       $"{patientOverviewItems.Type}");

                return new GetPatientDocumentResult.Success(patientOverviewItems);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving my record");
                return new GetPatientDocumentResult.BadGateway();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "My record retrieval return null body");
                return new GetPatientDocumentResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public Task<PatientDocument> GetPatientDocumentForDownload(GpUserSession gpUserSession, string documentIdentifier, string documentType,
            string documentName)
        {
            throw new NotImplementedException();
        }

        public byte[] ConvertDocumentToCorrectFormat(string type, string content)
        {
            throw new NotImplementedException();
        }

        public async Task<GetDetailedTestResult> GetDetailedTestResult(GpUserSession gpUserSession, string testResultId)
        {
            _logger.LogEnter();

            var tppUserSession = (TppUserSession)gpUserSession;

            try
            {
                _logger.LogDebug("Fetching TPP detailed test results");
                var detailedTestResult = await _tppClient.TestResultsViewDetailed(tppUserSession, testResultId);

                _logger.LogDebug($"Mapping TPP detailed test results to instance of {nameof(TestResultResponse)} class");
                var tppTestResultResponse = _patientDetailedTestResultChecker.Check(detailedTestResult);

                if (tppTestResultResponse.HasErrored)
                {
                    _logger.LogExitWith($"{nameof(tppTestResultResponse.HasErrored)}=true");
                    return new GetDetailedTestResult.BadGateway();
                }

                return new GetDetailedTestResult.Success(tppTestResultResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving test result");
                return new GetDetailedTestResult.BadGateway();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "Test Result retrieval return null body");
                return new GetDetailedTestResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<TestResults> GetLast180DaysTestResults(TppUserSession tppUserSession)
        {
            _logger.LogEnter();

            var tppTestResultDates = GetTestResultDateParams();
            var combinedTestResults = new List<TestResultItem>();

            _logger.LogDebug("Grouping test results by date");
            foreach (var testResultDates in tppTestResultDates)
            {
                var testResultsView = await _testResultsView.Post((tppUserSession,
                    testResultDates.StartDate.ToString(TestResultDateFormat, CultureInfo.InvariantCulture),
                    testResultDates.EndDate.ToString(TestResultDateFormat, CultureInfo.InvariantCulture)));

                _logger.LogDebug($"Mapping TPP test results to instance of {nameof(TestResults)} class");
                var testResults = _patientTestResultsChecker.Check(testResultsView);

                if (!testResults.HasAccess || testResults.HasErrored)
                {
                    _logger.LogExitWith($"{nameof(testResults.HasAccess)}={testResults.HasAccess}, {nameof(testResults.HasErrored)}={testResults.HasErrored}");
                    return testResults;
                }

                testResults.Data.ToList().ForEach(res => combinedTestResults.Add(res));
            }

            _logger.LogExit();
            return new TestResults { Data = combinedTestResults };
        }

        private static List<TppTestResultDates> GetTestResultDateParams()
        {

            var today = DateTime.Now.Date;

            return new List<TppTestResultDates>
            {
                new TppTestResultDates
                {
                    StartDate = today.AddDays(-179),
                    EndDate = today.AddDays(-120).AddHours(23).AddMinutes(59).AddSeconds(59)
                },
                new TppTestResultDates
                {
                    StartDate = today.AddDays(-119),
                    EndDate = today.AddDays(-60).AddHours(23).AddMinutes(59).AddSeconds(59),
                },
                new TppTestResultDates
                {
                    StartDate = today.AddDays(-59),
                    EndDate = today.AddHours(23).AddMinutes(59).AddSeconds(59),
                }
            };
        }

        private async Task<TppDcrItems> RetrievePatientRecordDcrItems(TppUserSession tppUserSession)
        {
            try
            {

                _logger.LogInformation("Retrieving PatientDocuments from DCR Events.");
                var patientRecord = await _requestPatientRecord.Post(tppUserSession);

                _logger.LogDebug($"Mapping TPP DCR responses to instance of {nameof(DocumentItem)} class");
                var dcrItems = new TppDcrItems
                {
                    PatientDocuments = _patientDocumentsFromDcrEventsTaskChecker.Check(patientRecord),
                    TppDcrEvents = _patientDcrEventsChecker.Check(patientRecord)
                };

                return dcrItems;


            }
            catch (Exception e)
            {

                _logger.LogError(e, "Retrieving dcr events failed. Returning hasErrored as true" +
                                    " for Documents and DCR events");
                return new TppDcrItems
                {
                    PatientDocuments = new PatientDocuments()
                    {
                        HasErrored = true
                    },
                    TppDcrEvents = new TppDcrEvents
                    {
                        HasErrored = true
                    }
                };
            }
        }

        private async Task<TestResults> RetrieveTestResults(TppUserSession tppUserSession)
        {
            try
            {

                _logger.LogInformation("Retrieving Test Results.");
                return await GetLast180DaysTestResults(tppUserSession);

            }
            catch (Exception e)
            {

                _logger.LogError(e, "Retrieving testResults failed. Returning hasErrored as true");
                return new TestResults
                {
                    HasErrored = true
                };
            }
        }

        private async Task<Tuple<Allergies, Medications>> RetrievePatientOverviewItems(TppUserSession tppUserSession)
        {
            try
            {
                var patientOverview = await _patientOverview.Post(tppUserSession);
                _logger.LogDebug($"Mapping TPP Patient Overview responses to lists of {nameof(Allergies)} and {nameof(Medications)} classes");
                return _patientOverviewTaskChecker.Check(patientOverview);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Retrieving patientOverViewItems failed. Returning HasErrored as true");
                var result = new Tuple<Allergies, Medications>(new Allergies(), new Medications());
                result.Item1.HasErrored = true;
                result.Item2.HasErrored = true;

                return result;
            }
        }

        private async Task<PatientDocument> RetrievePatientDocument(string documentIdentifier, TppUserSession tppUserSession)
        {
            _logger.LogEnter();
            var binaryDataRequest = new RequestBinaryData
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.UnitId,
                BinaryDataId = documentIdentifier
            };

            try
            {
                var patientOverview = await _tppClient.RequestBinaryData(binaryDataRequest, tppUserSession);
                _logger.LogExit();
                return _patientDocumentTaskChecker.Check(patientOverview);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Retrieving patientOverViewItems failed. Returning hasErrored as true");
                var result = new PatientDocument
                {
                    HasErrored = true
                };
                _logger.LogExit();
                return result;
            }
        }
    }
}