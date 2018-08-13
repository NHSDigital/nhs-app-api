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

        public Tuple<Allergies, Medications> Check(TppClient.TppApiObjectResponse<ViewPatientOverviewReply> taskResponse)
        {
            var methodName = "Check";
            _logger.LogDebug("Entered: {0}", methodName);
            
            var result = new Tuple<Allergies, Medications>(new Allergies(), new Medications());
            var hasErrored = false;
            var hasAccess = true;
            
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
                _logger.LogDebug("Mapping TPP response to allergies and medications");
                result = new TppPatientOverviewMapper(_logger).Map(taskResponse.Body);
            }

            _logger.LogDebug("Exiting: {0}", methodName);
            return result;
        }
    }
}
