using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class TppPatientRecordService : IPatientRecordService
    {
        private const string TestResultDateFormat = "yyyy-MM-ddTHH:mm:ss.f'Z'";
        private readonly ILogger<TppPatientRecordService> _logger;
        private readonly IGetPatientDcrEventsTaskChecker _patientDcrEventsChecker;
        private readonly IGetPatientOverviewTaskChecker _patientOverviewTaskChecker;
        private readonly IGetPatientTestResultsTaskChecker _patientTestResultsChecker;
        private readonly IGetTppDetailedTestResultChecker _patientDetailedTestResultChecker;
        private readonly ITppClient _tppClient;
        private readonly ITppMyRecordMapper _tppMyRecordMapper;
        
        public TppPatientRecordService(IGetPatientDcrEventsTaskChecker patientDcrEventsChecker,
            IGetPatientOverviewTaskChecker patientOverviewTaskChecker,
            IGetPatientTestResultsTaskChecker patientTestResultsChecker,
            IGetTppDetailedTestResultChecker patientDetailedTestResultChecker,
            ILogger<TppPatientRecordService> logger, ITppClient tppClient, ITppMyRecordMapper tppMyRecordMapper)
        {
            _patientDcrEventsChecker = patientDcrEventsChecker;
            _patientOverviewTaskChecker = patientOverviewTaskChecker;
            _patientTestResultsChecker = patientTestResultsChecker;
            _patientDetailedTestResultChecker = patientDetailedTestResultChecker;
            _tppClient = tppClient;
            _tppMyRecordMapper = tppMyRecordMapper;
            _logger = logger;
        }
        
        public async Task<GetMyRecordResult> GetMyRecord(GpUserSession gpUserSession)
        {
            _logger.LogEnter();
            
            var tppUserSession = (TppUserSession)gpUserSession;

            try
            {
                var patientOverview = await _tppClient.PatientOverviewPost(tppUserSession);
                var patientRecord = await _tppClient.RequestPatientRecordPost(tppUserSession);
                var testResults = await GetLast180DaysTestResults(tppUserSession);
                
                _logger.LogDebug($"Mapping TPP Patient Overview responses to lists of {nameof(Allergies)} and {nameof(Medications)} classes");
                var patientOverviewItems = _patientOverviewTaskChecker.Check(patientOverview);  
                
                var allergies = patientOverviewItems.Item1;
                var medications = patientOverviewItems.Item2;
                
                _logger.LogDebug($"Mapping TPP DCR responses to instance of {nameof(TppDcrEvents)} class" );
                var dcrEvents = _patientDcrEventsChecker.Check(patientRecord);

                _logger.LogInformation($"Mapping TPP responses to universal {nameof(MyRecordResponse)} class instance");
                var myRecordResponse = _tppMyRecordMapper.Map(allergies, medications, dcrEvents, testResults);
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

        public Task<GetPatientDocumentResult> GetPatientDocument(GpUserSession gpUserSession, string documentGuid,
            string documentType, string documentName)
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
                var testResultsView = await _tppClient.TestResultsView(tppUserSession, 
                    testResultDates.StartDate.ToString(TestResultDateFormat, CultureInfo.InvariantCulture), 
                    testResultDates.EndDate.ToString(TestResultDateFormat, CultureInfo.InvariantCulture));                  
                
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

        private List<TppTestResultDates> GetTestResultDateParams()
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
        
    }
}