using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.TaskChecker
{
    public class VisionGetAllergyTaskChecker
    {
        private readonly ILogger _logger;

        public VisionGetAllergyTaskChecker(ILogger logger)
        {
            _logger = logger;
        }

        public Allergies Check(Task<VisionClient.VisionApiObjectResponse<VisionPatientDataResponse>> task)
        {
            _logger.LogInformation("Checking Vision Allergies");

            var allergiesResult = task.Result;
            
            if (!allergiesResult.HasErrorResponse) 
                return new VisionAllergyMapper(_logger).Map(allergiesResult?.Body);
            
            if (allergiesResult.IsAccessDeniedError)
            {
                _logger.LogWarning("User does not have access to their patient record");
                return new Allergies
                {
                    HasAccess = false
                };
            }

            _logger.LogError($"Unsuccessful request retrieving allergy information for Vision. Status code: {(int)allergiesResult.StatusCode}");
            return new Allergies
            {
                HasErrored = true
            };

        }
    }
}
