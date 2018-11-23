using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class GetImmunisationsTaskChecker
    {
        private readonly ILogger<EmisPatientRecordService> _logger;
        
        public GetImmunisationsTaskChecker(ILogger<EmisPatientRecordService> logger)
        {
            _logger = logger;
        }
        
        public Immunisations Check(Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> task)
        {
            _logger.LogEnter();
            
            Immunisations immunisations = null;
            
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Retrieving immunisations task completed unsuccessfully");
                immunisations = new Immunisations
                {
                    HasErrored = true
                };
            }
            
            var immunisationsResponse = task.Result;
            
            if (!immunisationsResponse.HasSuccessStatusCode)
            {
                // User does not have access
                if (immunisationsResponse.HasForbiddenResponse() ||
                    immunisationsResponse.HasExceptionWithMessageContaining("Requested record access is disabled by the practice"))
                {
                    _logger.LogWarning("User does not have access to their patient record");
                    immunisations = new Immunisations
                    {
                        HasAccess = false
                    };
                }
                else
                {
                    _logger.LogError(
                        $"Unsuccessful request retrieving immunisations list for patient. Status code: {(int) immunisationsResponse.StatusCode}");
                    immunisations = new Immunisations
                    {
                        HasErrored = true
                    };
                }
            }

            _logger.LogExit();
            return immunisations ?? new EmisImmunisationMapper().Map(immunisationsResponse.Body);
        }
    }
}
