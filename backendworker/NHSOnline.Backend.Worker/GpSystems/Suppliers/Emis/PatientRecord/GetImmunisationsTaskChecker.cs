using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class GetImmunisationsTaskChecker
    {
        private readonly ILogger _logger;
        
        public GetImmunisationsTaskChecker(ILogger logger)
        {
            _logger = logger;
        }
        
        public Immunisations Check(Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> task)
        {
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
                if (immunisationsResponse.HasExceptionWithMessageContaining("Services Access violation") ||
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
            
            return immunisations ?? new EmisImmunisationMapper().Map(immunisationsResponse.Body);
        }
    }
}