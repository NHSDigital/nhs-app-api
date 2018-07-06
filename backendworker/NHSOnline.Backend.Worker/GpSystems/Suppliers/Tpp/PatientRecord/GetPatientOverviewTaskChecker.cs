using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using Allergies = NHSOnline.Backend.Worker.Areas.MyRecord.Models.Allergies;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class GetPatientOverviewTaskChecker
    {
        private readonly ILogger _logger;
        
        public GetPatientOverviewTaskChecker(ILogger logger)
        {
            _logger = logger;
        }

        public Tuple<Allergies, Medications> Check(Task<TppClient.TppApiObjectResponse<ViewPatientOverviewReply>> task)
        {
            var result = new Tuple<Allergies, Medications>(new Allergies(), new Medications());
            bool hasErrored = false;
            bool hasAccess = true;
            
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Retrieving patient overview task completed unsuccessfully");
                hasErrored = true;
            }
            
            var taskResponse = task.Result;
            
            if (!taskResponse.HasSuccessResponse)
            {
                // User does not have access
                if (taskResponse.HasForbiddenResponse)
                {
                    _logger.LogWarning("User does not have access to their patient record for Tpp");
                    hasAccess = false;
                }
                else
                {
                    _logger.LogError($"Unsuccessful request retrieving patient selected information for Tpp. Status code: {(int)taskResponse.StatusCode}");
                    hasErrored = true;
                }
            }

            if (hasErrored || !hasAccess)
            {
                result.Item1.HasErrored = hasErrored;
                result.Item1.HasAccess = hasAccess;
                result.Item2.HasErrored = hasErrored;
                result.Item2.HasAccess = hasAccess;
            }
            else
            {
                result = new TppPatientOverviewMapper().Map(taskResponse.Body);
            }

            return result;
        }
    }
}
