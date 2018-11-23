using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class GetConsultationsTaskChecker
    {
        private readonly ILogger<EmisPatientRecordService> _logger;
        
        public GetConsultationsTaskChecker(ILogger<EmisPatientRecordService> logger)
        {
            _logger = logger;
        }
        
        public Consultations Check(Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> task)
        {
            _logger.LogEnter();
            
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

            _logger.LogExit();
            return consultations ?? new EmisConsulationMapper().Map(consultationsResponse.Body);
        }
    }
}
