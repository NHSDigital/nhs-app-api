using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class GetPatientDcrEvents
    {
        private readonly ILogger _logger;
        
        public GetPatientDcrEvents(ILogger logger)
        {
            _logger = logger;
        }

        public TppDcrEvents Check(Task<TppClient.TppApiObjectResponse<RequestPatientRecordReply>> task)
        {
            TppDcrEvents tppDcrEvents = null;
 
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Retrieving patient record task completed unsuccessfully");
                tppDcrEvents = new TppDcrEvents()
                {
                    HasErrored = true
                };
            }
            
            var taskResponse = task.Result;

            if (taskResponse.HasSuccessResponse) 
                return tppDcrEvents ?? new TppDcrEventsMapper().Map(taskResponse.Body);
            
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

            return tppDcrEvents;
        }
    }
}
