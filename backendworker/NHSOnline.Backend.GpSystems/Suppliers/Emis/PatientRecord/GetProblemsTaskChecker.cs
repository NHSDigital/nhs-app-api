using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class GetProblemsTaskChecker
    {
        private readonly ILogger<GetProblemsTaskChecker> _logger;
        private readonly EmisProblemMapper _mapper;
        
        public GetProblemsTaskChecker(ILogger<GetProblemsTaskChecker> logger, EmisProblemMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }
        
        public Problems Check(Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> task)
        {
            _logger.LogEnter();
            
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
            
            if (!problemsResponse.HasSuccessResponse)
            {
                // User does not have access
                if (problemsResponse.HasForbiddenResponse() ||
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

            _logger.LogExit();
            return problems ?? _mapper.Map(problemsResponse.Body);
        }      
    }
}
