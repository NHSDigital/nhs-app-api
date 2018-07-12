using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class GetPatientTestResultsTaskChecker
    {
        private readonly ILogger _logger;
        
        public GetPatientTestResultsTaskChecker(ILogger logger)
        {
            _logger = logger;
        }

        public TestResults Check(TppClient.TppApiObjectResponse<TestResultsViewReply> taskResponse)
        {
            TestResults testResults = null;
            if (taskResponse.HasSuccessResponse) 
                return new TppTestResultsMapper().Map(taskResponse.Body);
            
            if (taskResponse.HasForbiddenResponse)
            {
                _logger.LogWarning("User does not have access to their test results for Tpp");
                testResults = new TestResults
            {
                    HasAccess = false
                };
            }
            else
            {
                _logger.LogError($"Unsuccessful request retrieving test result information for Tpp. Status code: {(int)taskResponse.StatusCode}");
                testResults = new TestResults
                {
                    HasErrored = true
                };
            }

            return testResults;
        }
    }
}
