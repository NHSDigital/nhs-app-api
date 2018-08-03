using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class GetAllergiesTaskChecker
    {
        private readonly ILogger _logger;
        
        public GetAllergiesTaskChecker(ILogger logger)
        {
            _logger = logger;
        }
        
        public Allergies Check(Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> task)
        {
            var methodName = "Check";
            _logger.LogDebug("Entered: {0}", methodName);
            
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
            
            if (!allergiesResponse.HasSuccessStatusCode)
            {
                // User does not have access
                if (allergiesResponse.HasExceptionWithMessageContaining("Services Access violation") ||
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
            
            _logger.LogDebug("Exiting: {0}", methodName);
            return allergies ?? new EmisAllergyMapper().Map(allergiesResponse.Body);
        }
    }
}