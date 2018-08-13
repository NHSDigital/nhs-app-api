using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;

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
            var methodName = "Check";
            _logger.LogDebug("Entered: {0}", methodName);
            TppDcrEvents tppDcrEvents = null;

            if (taskResponse.HasSuccessResponse)
            {              
                _logger.LogDebug("Exiting: {0} with HasSuccessResponse=true", methodName);
                return tppDcrEvents ?? new TppDcrEventsMapper().Map(taskResponse.Body);
            }
            
            if (taskResponse.HasForbiddenResponse)
            {
                _logger.LogWarning("User does not have access to their patient record for Tpp");
                tppDcrEvents = new TppDcrEvents()
                {
                    HasAccess = false
                };
            }
            else
            {
                _logger.LogError($"Unsuccessful request retrieving patient record information for Tpp. Status code: {(int)taskResponse.StatusCode}");
                tppDcrEvents = new TppDcrEvents()
                {
                    HasErrored = true
                };
            }

            _logger.LogDebug("Exiting: {0}", methodName);
            return tppDcrEvents;
        }
    }
}
