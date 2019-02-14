using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
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
            _logger.LogEnter();

            if (taskResponse.HasSuccessResponse)
            {
                _logger.LogDebug($"Mapping TPP {nameof(TestResultsViewReply)} response to instance of standardised {nameof(TestResultResponse)} class");
                _logger.LogExitWith($"{nameof(taskResponse.HasSuccessResponse)}=true");
                                                
                return _testResultsMapper.Map(taskResponse.Body);             
            }
            
            _logger.LogError($"Unsuccessful request retrieving test result information for Tpp. Status code: {(int)taskResponse.StatusCode}");

            _logger.LogExit();
            return new TestResultResponse { HasErrored = true };
        }
    }
}
