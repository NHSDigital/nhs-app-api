using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface IGetTppDetailedTestResultChecker
    {
        TestResultResponse Check(TppClient.TppApiObjectResponse<TestResultsViewReply> taskResponse);
    }
    
    public class GetTppDetailedTestResultChecker : IGetTppDetailedTestResultChecker
    {
        private readonly ITppDetailedTestResultMapper _testResultsMapper;
        private readonly ILogger<GetTppDetailedTestResultChecker> _logger;
        
        public GetTppDetailedTestResultChecker(ITppDetailedTestResultMapper testResultsMapper, ILogger<GetTppDetailedTestResultChecker> logger)
        {
            _testResultsMapper = testResultsMapper;
            _logger = logger;
        }

        public TestResultResponse Check(TppClient.TppApiObjectResponse<TestResultsViewReply> taskResponse)
        {
            var methodName = "Check";
            _logger.LogEnter();

            if (taskResponse.HasSuccessResponse)
            {
                _logger.LogDebug("Mapping TPP TestResultsViewReply response to instance of standardised TestResultResponse class");
                _logger.LogDebug("Exiting: {0} with HasSuccessResponse=true", methodName);
                return _testResultsMapper.Map(taskResponse.Body);             
            }
            
            _logger.LogError($"Unsuccessful request retrieving test result information for Tpp. Status code: {(int)taskResponse.StatusCode}");

            _logger.LogExit();
            return new TestResultResponse { HasErrored = true };
        }
    }
}
