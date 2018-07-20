using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class GetTppDetailedTestResultChecker
    {
        private readonly ILogger _logger;
        
        public GetTppDetailedTestResultChecker(ILogger logger)
        {
            _logger = logger;
        }

        public TestResultResponse Check(TppClient.TppApiObjectResponse<TestResultsViewReply> taskResponse)
        {
            if (taskResponse.HasSuccessResponse) 
                return new TppDetailedTestResultMapper().Map(taskResponse.Body);
            
            _logger.LogError($"Unsuccessful request retrieving test result information for Tpp. Status code: {(int)taskResponse.StatusCode}");

            return new TestResultResponse { HasErrored = true };
        }
    }
}
