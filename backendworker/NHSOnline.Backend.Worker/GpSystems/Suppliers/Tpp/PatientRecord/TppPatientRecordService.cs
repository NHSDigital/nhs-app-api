using System;
using System.Collections.Generic;
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
        private const string TestResultDateFormat = "yyyy-MM-ddTHH:mm:ssz'Z'";
        private readonly ILogger _logger;
        private readonly ITppClient _tppClient;
        private readonly ITppMyRecordMapper _tppMyRecordMapper;
        
        public TppPatientRecordService(ILoggerFactory loggerFactory, ITppClient tppClient, ITppMyRecordMapper tppMyRecordMapper)
        {
            _tppClient = tppClient;
            _tppMyRecordMapper = tppMyRecordMapper;
            _logger = loggerFactory.CreateLogger<TppPatientRecordService>();
        }
        
        public async Task<GetMyRecordResult> Get(UserSession userSession)
        {
            var tppUserSession = (TppUserSession) userSession;

            try
            {               
                var patientOverview = await _tppClient.PatientOverviewPost(tppUserSession);
                var patientRecord = await _tppClient.RequestPatientRecordPost(tppUserSession);
                var testResults = await GetLast180DaysTestResults(tppUserSession);

                var patientOverviewItems = new GetPatientOverviewTaskChecker(_logger).Check(patientOverview);  
                
                var allergies = patientOverviewItems.Item1;
                var medications = patientOverviewItems.Item2;
                
                var dcrEvents = new GetPatientDcrEventsTaskChecker(_logger).Check(patientRecord);

                var myRecordResponse = _tppMyRecordMapper.Map(allergies, medications, dcrEvents, testResults);
                myRecordResponse.Supplier = userSession.Supplier.ToString().ToUpper();
                
                _logger.LogInformation("MyRecordResponse: " + myRecordResponse);

                return new GetMyRecordResult.SuccessfullyRetrieved(myRecordResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving my record");
                return new GetMyRecordResult.Unsuccessful();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "My record retrieval return null body");
                return new GetMyRecordResult.SupplierBadData();
            }
        }

        private async Task<TestResults> GetLast180DaysTestResults(TppUserSession tppUserSession)
        {           
            var today = DateTime.Now;
            var tppTestResultDates = new List<TppTestResultDates>
            {
                new TppTestResultDates
                {
                    StartDate = today.AddDays(-180),
                    EndDate = today.AddDays(-120)                  
                },
                new TppTestResultDates
                {
                    StartDate = today.AddDays(-120).AddMilliseconds(1),
                    EndDate = today.AddDays(-60),
                },
                new TppTestResultDates
                {
                    StartDate = today.AddDays(-60).AddMilliseconds(1),
                    EndDate = today
                }
            };

            var combinedTestResults = new List<TestResultItem>();
            
            foreach (var testResultDates in tppTestResultDates)
            {               
                var testResultsView = await _tppClient.TestResultsView(tppUserSession, 
                    testResultDates.StartDate.ToString(TestResultDateFormat), 
                    testResultDates.EndDate.ToString(TestResultDateFormat));
                                
                var testResults = new GetPatientTestResultsTaskChecker(_logger).Check(testResultsView);

                if (!testResults.HasAccess || testResults.HasErrored)
                    return testResults;
                                   
                testResults.Data.ToList().ForEach(res => combinedTestResults.Add(res));
            }

            return new TestResults { Data = combinedTestResults };
        }
    }
}