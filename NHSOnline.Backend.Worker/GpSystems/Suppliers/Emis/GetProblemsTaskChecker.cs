using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord.Model;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class GetProblemsTaskChecker
    {
        private readonly ILogger _logger;
        
        public GetProblemsTaskChecker(ILogger logger)
        {
            _logger = logger;
        }
        
        public Problems Check(Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> task)
        {
            Problems problems = null;
            
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Retrieving problems task completed unsuccessfully");
                problems = new Problems
                {
                    HasErrored = true
                };
            }
            
            var problemsResponse = task.Result;
            
            if (!problemsResponse.HasSuccessStatusCode)
            {
                // User does not have access
                if (problemsResponse.HasExceptionWithMessageContaining("Services Access violation") ||
                    problemsResponse.HasExceptionWithMessageContaining("Requested record access is disabled by the practice"))
                {
                    _logger.LogWarning("User does not have access to their patient record");
                    problems = new Problems
                    {
                        HasAccess = false
                    };
                }
                else
                {
                    _logger.LogError(
                        $"Unsuccessful request retrieving Problem list for patient. Status code: {(int) problemsResponse.StatusCode}");
                    problems = new Problems
                    {
                        HasErrored = true
                    };
                }
            }
            
            return problems ?? new EmisProblemMapper().Map(problemsResponse.Body);
        }      
    }
}