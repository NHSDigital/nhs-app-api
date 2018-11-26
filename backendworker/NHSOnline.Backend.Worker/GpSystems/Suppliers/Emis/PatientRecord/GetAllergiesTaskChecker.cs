using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class GetAllergiesTaskChecker
    {
        private readonly ILogger<EmisPatientRecordService> _logger;
        
        public GetAllergiesTaskChecker(ILogger<EmisPatientRecordService> logger)
        {
            _logger = logger;
        }
        
        public Allergies Check(Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> task)
        {
            _logger.LogEnter();
            
            Allergies allergies = null;
            
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Retrieving allergies task completed unsuccessfully");
                allergies = new Allergies
                {
                    HasErrored = true
                };
            }
            
            var allergiesResponse = task.Result;
            
            if (!allergiesResponse.HasSuccessResponse)
            {
                // User does not have access
                if (allergiesResponse.HasForbiddenResponse() ||
                    allergiesResponse.HasExceptionWithMessageContaining("Requested record access is disabled by the practice"))
                {
                    _logger.LogWarning("User does not have access to their patient record");
                    allergies = new Allergies
                    {
                        HasAccess = false
                    };
                }
                else
                {
                    _logger.LogError(
                        $"Unsuccessful request retrieving Allergy list for patient. Status code: {(int) allergiesResponse.StatusCode}");
                    allergies = new Allergies
                    {
                        HasErrored = true
                    };
                }
            }

            _logger.LogExit();
            return allergies ?? new EmisAllergyMapper().Map(allergiesResponse.Body);
        }
    }
}
