using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
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
        
        public async Task<GetMyRecordResult> GetMyRecord(UserSession userSession)
        {
            var methodName = "GetMyRecord";
            _logger.LogDebug("Entered: {0}", methodName);
            
            var tppUserSession = (TppUserSession) userSession;

            try
            {               
                var patientOverview = await _tppClient.PatientOverviewPost(tppUserSession);
                var patientRecord = await _tppClient.RequestPatientRecordPost(tppUserSession);
                var testResults = await GetLast180DaysTestResults(tppUserSession);

                _logger.LogDebug("Mapping TPP Patient Overview responses to lists of Allergies and Medication classes");
                var patientOverviewItems = _patientOverviewTaskChecker.Check(patientOverview);  
                
                var allergies = patientOverviewItems.Item1;
                var medications = patientOverviewItems.Item2;
                
                _logger.LogDebug("Mapping TPP DCR responses to instnace of TppDcrEvents class");
                var dcrEvents = _patientDcrEventsChecker.Check(patientRecord);

                _logger.LogInformation("Mapping TPP responses to universal MyRecordResponse class instance");
                var myRecordResponse = _tppMyRecordMapper.Map(allergies, medications, dcrEvents, testResults);
                myRecordResponse.Supplier = userSession.Supplier.ToString().ToUpper(CultureInfo.InvariantCulture);
                
                _logger.LogInformation("MyRecordResponse: " + myRecordResponse);

                _logger.LogDebug("Exiting: {0}", methodName);
                return new GetMyRecordResult.SuccessfullyRetrieved(myRecordResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving my record");
                _logger.LogDebug("Exiting: {0}", methodName);
                return new GetMyRecordResult.Unsuccessful();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "My record retrieval return null body");
                _logger.LogDebug("Exiting: {0}", methodName);
                return new GetMyRecordResult.SupplierBadData();
            }
        }

        public async Task<GetDetailedTestResult> GetDetailedTestResult(UserSession userSession, string testResultId)
        {
            var methodName = "GetDetailedTestResult";
            _logger.LogDebug("Entered: {0}", methodName);
            
            var tppUserSession = (TppUserSession) userSession;

            try
            {           
                _logger.LogDebug("Fetching TPP detailed test results");    
                var detailedTestResult = await _tppClient.TestResultsViewDetailed(tppUserSession, testResultId);

                _logger.LogDebug("Mapping TPP detailed test results to instance of TestResultResponse class");   
                var tppTestResultResponse = _patientDetailedTestResultChecker.Check(detailedTestResult);

                if (tppTestResultResponse.HasErrored)
                {
                    _logger.LogDebug("Exiting: {0}, HasErrored=true", methodName);
                    return new GetDetailedTestResult.Unsuccessful();
                }

                _logger.LogDebug("Exiting: {0}", methodName);
                return new GetDetailedTestResult.SuccessfullyRetrieved(tppTestResultResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving test result");
                _logger.LogDebug("Exiting: {0}", methodName);
                return new GetDetailedTestResult.Unsuccessful();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "Test Result retrieval return null body");
                _logger.LogDebug("Exiting: {0}", methodName);
                return new GetDetailedTestResult.SupplierBadData();
            }
        }
        
        private async Task<TestResults> GetLast180DaysTestResults(TppUserSession tppUserSession)
        {   
            var methodName = "GetLast180DaysTestResults";
            _logger.LogDebug("Entered: {0}", methodName);
            
            var tppTestResultDates = GetTestResultDateParams();
            var combinedTestResults = new List<TestResultItem>();
            
            _logger.LogDebug("Grouping test results by date");
            foreach (var testResultDates in tppTestResultDates)
            {               
                var testResultsView = await _tppClient.TestResultsView(tppUserSession, 
                    testResultDates.StartDate.ToString(TestResultDateFormat, CultureInfo.InvariantCulture), 
                    testResultDates.EndDate.ToString(TestResultDateFormat, CultureInfo.InvariantCulture));                  
                
                _logger.LogDebug("Mapping TPP test results to instance of TestResults class");
                var testResults = _patientTestResultsChecker.Check(testResultsView);

                if (!testResults.HasAccess || testResults.HasErrored)
                {
                    _logger.LogDebug("Exiting: {0}, HasAccess={1}, HasErrored={2}", methodName, testResults.HasAccess, testResults.HasErrored);
                    return testResults;                
                }
                                   
                testResults.Data.ToList().ForEach(res => combinedTestResults.Add(res));
            }

            _logger.LogDebug("Exiting: {0}", methodName);
            return new TestResults { Data = combinedTestResults };
        }

        private List<TppTestResultDates> GetTestResultDateParams()
        {
            var methodName = "GetTestResultDateParams";
            _logger.LogDebug("Entered: {0}", methodName);
            
            var today = DateTime.Now.Date;
            
            _logger.LogDebug("Exiting: {0}", methodName);
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