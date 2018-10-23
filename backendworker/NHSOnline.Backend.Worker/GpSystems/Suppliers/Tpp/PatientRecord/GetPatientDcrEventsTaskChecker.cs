using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class GetPatientDcrEventsTaskChecker
    {
        private readonly ILogger _logger;
        
        public GetPatientDcrEventsTaskChecker(ILogger logger)
        {
            _logger = logger;
        }

        public TppDcrEvents Check(TppClient.TppApiObjectResponse<RequestPatientRecordReply> taskResponse)
        {
            _logger.LogDebug("Entered: {0}", nameof(Check));

            if (taskResponse.HasSuccessResponse)
            {              
                _logger.LogDebug("Exiting: {0} with HasSuccessResponse=true", nameof(Check));
                return new TppDcrEventsMapper().Map(taskResponse.Body);
            }

            TppDcrEvents tppDcrEvents;

            if (taskResponse.HasForbiddenResponse)
            {
                _logger.LogWarning("User does not have access to their patient record for Tpp");
                tppDcrEvents = new TppDcrEvents
                {
                    HasAccess = false
                };
            }
            else
            {
                _logger.LogError($"Unsuccessful request retrieving patient record information for Tpp. Status code: {(int)taskResponse.StatusCode}");
                tppDcrEvents = new TppDcrEvents
                {
                    HasErrored = true
                };
            }

            _logger.LogDebug("Exiting: {0}", nameof(Check));
            return tppDcrEvents;
        }
    }
}
