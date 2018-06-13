using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class GetMedicationsTaskChecker
    {
        private readonly ILogger _logger;
        
        public GetMedicationsTaskChecker(ILogger logger)
        {
            _logger = logger;
        }
        
        public Medications Check(Task<EmisClient.EmisApiObjectResponse<MedicationRequestsGetResponse>> task)
        {
            Medications medications = null;
            
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Retrieving medications task completed unsuccessfully");
                medications = new Medications
                {
                    HasErrored = true
                };
            }
            
            var medicationsResponse = task.Result;
            
            if (!medicationsResponse.HasSuccessStatusCode)
            {
                // User does not have access
                if (medicationsResponse.HasExceptionWithMessageContaining("Services Access violation"))
                {
                    _logger.LogWarning("User does not have access to their patient record");
                    medications = new Medications
                    {
                        HasAccess = false
                    };
                }
                else
                {
                    _logger.LogError(
                        $"Unsuccessful request retrieving medications list for patient. Status code: {(int) medicationsResponse.StatusCode}");
                    medications = new Medications
                    {
                        HasErrored = true
                    };
                }
            }
            
            return medications ?? new EmisMedicationMapper().Map(medicationsResponse.Body);
        }
    }
}