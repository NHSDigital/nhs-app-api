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
        private readonly ITppMyRecordMapper _tppMyRecordMapper;
        private readonly ITppClientRequest<TppRequestParameters, ViewPatientOverviewReply> _patientOverview;
        private readonly ITppClientRequest<TppRequestParameters, RequestPatientRecordReply> _requestPatientRecord;
        private readonly ITppClientRequest<(TppRequestParameters tppRequestParameters, string startDate, string endDate), TestResultsViewReply> _testResultsView;
        private readonly ITppClientRequest<(TppRequestParameters tppRequestParameters, string documentIdentifier), RequestBinaryDataReply> _requestBinaryData;
        private readonly ITppClientRequest<(TppRequestParameters tppRequestParameters, string testResultId), TestResultsViewReply> _testResultsViewDetailed;

        public TppPatientRecordService(IGetPatientDcrEventsTaskChecker patientDcrEventsChecker,
            IGetPatientOverviewTaskChecker patientOverviewTaskChecker,
            IGetPatientDocumentTaskChecker patientDocumentTaskChecker,
            IGetPatientTestResultsTaskChecker patientTestResultsChecker,
            IGetTppDetailedTestResultChecker patientDetailedTestResultChecker,
            ITppClientRequest<TppRequestParameters, ViewPatientOverviewReply> patientOverview,
            ITppClientRequest<TppRequestParameters, RequestPatientRecordReply> requestPatientRecord,
            ITppClientRequest<(TppRequestParameters tppRequestParameters, string startDate, string endDate), TestResultsViewReply> testResultsView,
            ITppClientRequest<(TppRequestParameters tppRequestParameters, string documentIdentifier), RequestBinaryDataReply> requestBinaryData,
            ITppClientRequest<(TppRequestParameters tppRequestParameters, string testResultId), TestResultsViewReply> testResultsViewDetailed,
            IGetPatientDocumentsFromDcrEventsTaskChecker patientDocumentsFromDcrEventsTaskChecker,
            ILogger<TppPatientRecordService> logger, ITppMyRecordMapper tppMyRecordMapper)
        {
            _patientDcrEventsChecker = patientDcrEventsChecker;
            _patientOverviewTaskChecker = patientOverviewTaskChecker;
            _patientDocumentTaskChecker = patientDocumentTaskChecker;
            _patientTestResultsChecker = patientTestResultsChecker;
            _patientDetailedTestResultChecker = patientDetailedTestResultChecker;
            _patientOverview = patientOverview;
            _patientDocumentsFromDcrEventsTaskChecker = patientDocumentsFromDcrEventsTaskChecker;
            _tppMyRecordMapper = tppMyRecordMapper;
            _requestPatientRecord = requestPatientRecord;
            _testResultsView = testResultsView;
            _requestBinaryData = requestBinaryData;
            _testResultsViewDetailed = testResultsViewDetailed;
            _logger = logger;
        }

        public async Task<GetMyRecordResult> GetMyRecord(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            try
            {
                var tppRequestParameters = gpLinkedAccountModel.BuildTppRequestParameters(_logger);

                // TPP does not yet handle concurrent requests
                var patientOverviewItems = await RetrievePatientOverviewItems(tppRequestParameters);
                var dcrItems = await RetrievePatientRecordDcrItems(tppRequestParameters);
                var testResults = await RetrieveTestResults(tppRequestParameters);

                var documentItems = dcrItems.PatientDocuments;
                var dcrEvents = dcrItems.TppDcrEvents;

                _logger.LogInformation($"Number of documents for user " +
                                       $"at ODSCode {tppRequestParameters.OdsCode} is " +
                                       $"{documentItems.Data.Count()}");

                var allergies = patientOverviewItems.Item1;
                var medications = patientOverviewItems.Item2;

                _logger.LogInformation($"Mapping TPP responses to universal {nameof(MyRecordResponse)} class instance");
                var myRecordResponse = _tppMyRecordMapper.Map(allergies, medications,
                    dcrEvents,
                    testResults,
                    documentItems);

                myRecordResponse.Supplier = Supplier.Tpp.ToString().ToUpper(CultureInfo.InvariantCulture);

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

        public async Task<GetPatientDocumentResult> GetPatientDocument(
            GpLinkedAccountModel gpLinkedAccountModel, string documentIdentifier, string documentType, string documentName)
        {
            _logger.LogEnter();

            try
            {
                var tppRequestParameters = gpLinkedAccountModel.BuildTppRequestParameters(_logger);
                var documentBinaryData = await RetrievePatientDocument(documentIdentifier, tppRequestParameters);

                var patientDocument = _patientDocumentTaskChecker.CheckForViewing(documentBinaryData);

                _logger.LogInformation(
                    $"Document type for user document at ODSCode " +
                    $"{gpLinkedAccountModel.GpUserSession.OdsCode} is {patientDocument.Type}");

                return new GetPatientDocumentResult.Success(patientDocument);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving patient document");
                return new GetPatientDocumentResult.BadGateway();
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, "Request for patient document returned null body");
                return new GetPatientDocumentResult.BadGateway();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Exception thrown when retrieving patient document");
                return new GetPatientDocumentResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetPatientDocumentDownloadResult> GetPatientDocumentForDownload(
            GpLinkedAccountModel gpLinkedAccountModel, string documentIdentifier, string documentType, string documentName)
        {
            try
            {
                _logger.LogEnter();

                var tppRequestParameters = gpLinkedAccountModel.BuildTppRequestParameters(_logger);
                var documentBinaryData = await RetrievePatientDocument(documentIdentifier, tppRequestParameters);

                var fileContentResult = _patientDocumentTaskChecker.CheckForDownload(documentBinaryData, documentName);

                if (fileContentResult is null)
                {
                    return new GetPatientDocumentDownloadResult.BadGateway();
                }

                return new GetPatientDocumentDownloadResult.Success(fileContentResult);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving patient document for download");
                return new GetPatientDocumentDownloadResult.BadGateway();
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, "Request for patient document for download returned null body");
                return new GetPatientDocumentDownloadResult.BadGateway();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Exception thrown when retrieving patient document for download");
                return new GetPatientDocumentDownloadResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetDetailedTestResult> GetDetailedTestResult(GpLinkedAccountModel gpLinkedAccountModel, string testResultId)
        {
            _logger.LogEnter();

            try
            {
                var tppRequestParameters = gpLinkedAccountModel.BuildTppRequestParameters(_logger);

                _logger.LogDebug("Fetching TPP detailed test results");
                var detailedTestResult = await _testResultsViewDetailed.Post((tppRequestParameters, testResultId));

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

        private async Task<TestResults> GetLast180DaysTestResults(TppRequestParameters tppRequestParameters)
        {
            _logger.LogEnter();

            var tppTestResultDates = GetTestResultDateParams();
            var combinedTestResults = new List<TestResultItem>();

            _logger.LogDebug("Grouping test results by date");
            foreach (var testResultDates in tppTestResultDates)
            {
                var testResultsView = await _testResultsView.Post((tppRequestParameters,
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

        private async Task<TppDcrItems> RetrievePatientRecordDcrItems(TppRequestParameters tppRequestParameters)
        {
            try
            {
                _logger.LogInformation("Retrieving PatientDocuments from DCR Events.");
                var patientRecord = await _requestPatientRecord.Post(tppRequestParameters);

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

        private async Task<TestResults> RetrieveTestResults(TppRequestParameters tppRequestParameters)
        {
            try
            {
                _logger.LogInformation("Retrieving Test Results.");

                return await GetLast180DaysTestResults(tppRequestParameters);
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

        private async Task<Tuple<Allergies, Medications>> RetrievePatientOverviewItems(TppRequestParameters tppRequestParameters)
        {
            try
            {
                var patientOverview = await _patientOverview.Post(tppRequestParameters);
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


        private async Task<TppApiObjectResponse<RequestBinaryDataReply>> RetrievePatientDocument(
            string documentIdentifier, TppRequestParameters tppRequestParameters)
        {
            _logger.LogEnter();

            try
            {
                return await _requestBinaryData.Post((tppRequestParameters, documentIdentifier));
            }
            finally
            {
                _logger.LogExit();
            }
        }

    }
}