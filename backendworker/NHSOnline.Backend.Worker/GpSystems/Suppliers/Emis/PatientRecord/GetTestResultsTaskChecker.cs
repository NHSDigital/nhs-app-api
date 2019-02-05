using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class GetTestResultsTaskChecker
    {
        private readonly ILogger<GetTestResultsTaskChecker> _logger;
        EmisTestResultMapper _mapper;
        
        public GetTestResultsTaskChecker(ILogger<GetTestResultsTaskChecker> logger, EmisTestResultMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }
        
        public TestResults Check(Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> task)
        {
            _logger.LogEnter();
            
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
            
            if (!testResultsResponse.HasSuccessResponse)
            {
                // User does not have access
                if (testResultsResponse.HasForbiddenResponse() ||
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

            _logger.LogExit();
            return testResults ?? _mapper.Map(testResultsResponse.Body);
        }
    }
}
