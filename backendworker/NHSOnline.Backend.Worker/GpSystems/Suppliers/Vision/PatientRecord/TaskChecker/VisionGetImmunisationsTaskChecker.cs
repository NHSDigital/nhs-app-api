using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.TaskChecker
{
    public class VisionGetImmunisationsTaskChecker
    {
        readonly ILogger _logger;

        public VisionGetImmunisationsTaskChecker(ILogger logger)
        {
            _logger = logger;
        }

        public Immunisations Check(Task<VisionClient.VisionApiObjectResponse<VisionPatientDataResponse>> task)
        {
            _logger.LogInformation("Checking Vision Immunisations");

            var immunisationsResult = task.Result;
            
            if (!immunisationsResult.HasErrorResponse) 
                return new VisionImmunisationsMapper(_logger).Map(immunisationsResult?.Body);
            
            if (immunisationsResult.IsAccessDeniedError)
            {
                _logger.LogWarning("User does not have access to their patient record");
                return new Immunisations
                {
                    HasAccess = false
                };
            }

            _logger.LogError($"Unsuccessful request retrieving Immunisations information for Vision. Status code: {(int)immunisationsResult.StatusCode}");
            return new Immunisations
            {
                HasErrored = true
            };

        }
    }
}
