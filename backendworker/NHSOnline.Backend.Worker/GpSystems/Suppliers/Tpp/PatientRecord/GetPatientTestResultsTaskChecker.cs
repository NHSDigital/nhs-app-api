using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface IGetPatientTestResultsTaskChecker
    {
        TestResults Check(TppClient.TppApiObjectResponse<TestResultsViewReply> taskResponse);
    }
    
    public class GetPatientTestResultsTaskChecker : IGetPatientTestResultsTaskChecker
    {
        private readonly ITppTestResultsMapper _testResultsMapper;
        private readonly ILogger<GetPatientTestResultsTaskChecker> _logger;
        
        public GetPatientTestResultsTaskChecker(ITppTestResultsMapper testResultsMapper, ILogger<GetPatientTestResultsTaskChecker> logger)
        {
            _testResultsMapper = testResultsMapper;
            _logger = logger;
        }

        public TestResults Check(TppClient.TppApiObjectResponse<TestResultsViewReply> taskResponse)
        {
            _logger.LogEnter();
            
            TestResults testResults;
            if (taskResponse.HasSuccessResponse)
            {            
                _logger.LogExitWith($"{nameof(taskResponse.HasSuccessResponse)}=true");
                return _testResultsMapper.Map(taskResponse.Body);
            }
            
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

            _logger.LogExit();
            return testResults;
        }
    }
}
