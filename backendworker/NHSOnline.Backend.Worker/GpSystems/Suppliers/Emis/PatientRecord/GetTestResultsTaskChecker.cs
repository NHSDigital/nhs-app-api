using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class GetTestResultsTaskChecker
    {
        private readonly ILogger _logger;
        
        public GetTestResultsTaskChecker(ILogger logger)
        {
            _logger = logger;
        }
        
        public TestResults Check(Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> task)
        {
            TestResults testResults = null;
            
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Retrieving test results task completed unsuccessfully");
                testResults = new TestResults
                {
                    HasErrored = true
                };
            }
            
            var testResultsResponse = task.Result;
            
            if (!testResultsResponse.HasSuccessStatusCode)
            {
                // User does not have access
                if (testResultsResponse.HasExceptionWithMessageContaining("Services Access violation") ||
                    testResultsResponse.HasExceptionWithMessageContaining("Requested record access is disabled by the practice"))
                {
                    _logger.LogWarning("User does not have access to their patient record");
                    testResults = new TestResults
                    {
                        HasAccess = false
                    };
                }
                else
                {
                    _logger.LogError(
                        $"Unsuccessful request retrieving test results list for patient. Status code: {(int) testResultsResponse.StatusCode}");
                    testResults = new TestResults
                    {
                        HasErrored = true
                    };
                }
            }
            
            return testResults ?? new EmisTestResultMapper().Map(testResultsResponse.Body);
        }
    }
}