using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class GetConsultationsTaskChecker
    {
        private readonly ILogger _logger;
        
        public GetConsultationsTaskChecker(ILogger logger)
        {
            _logger = logger;
        }
        
        public Consultations Check(Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> task)
        {
            var methodName = "Check";
            _logger.LogDebug("Entered: {0}", methodName);
            
            Consultations consultations = null;
            
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Retrieving consultations task completed unsuccessfully");
                consultations = new Consultations
                {
                    HasErrored = true
                };
            }
            
            var consultationsResponse = task.Result;
            
            if (!consultationsResponse.HasSuccessStatusCode)
            {
                // User does not have access
                if (consultationsResponse.HasForbiddenResponse() ||
                    consultationsResponse.HasExceptionWithMessageContaining("Requested record access is disabled by the practice"))
                {
                    _logger.LogWarning("User does not have access to their patient record");
                    consultations = new Consultations
                    {
                        HasAccess = false
                    };
                }
                else
                {
                    _logger.LogError(
                        $"Unsuccessful request retrieving consultations list for patient. Status code: {(int) consultationsResponse.StatusCode}");
                    consultations = new Consultations
                    {
                        HasErrored = true
                    };
                }
            }
            
            _logger.LogDebug("Exiting: {0}", methodName);
            return consultations ?? new EmisConsulationMapper().Map(consultationsResponse.Body);
        }
    }
}
