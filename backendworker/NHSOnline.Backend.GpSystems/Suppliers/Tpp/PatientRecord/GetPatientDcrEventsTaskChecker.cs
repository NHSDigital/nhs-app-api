using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface IGetPatientDcrEventsTaskChecker
    {
        TppDcrEvents Check(TppClient.TppApiObjectResponse<RequestPatientRecordReply> taskResponse);
    }
    
    public class GetPatientDcrEventsTaskChecker : IGetPatientDcrEventsTaskChecker
    {
        private readonly ITppDcrEventsMapper _dcrEventsMapper;
        private readonly ILogger<IGetPatientDcrEventsTaskChecker> _logger;
        
        public GetPatientDcrEventsTaskChecker(ITppDcrEventsMapper dcrEventsMapper, ILogger<GetPatientDcrEventsTaskChecker> logger)
        {
            _dcrEventsMapper = dcrEventsMapper;
            _logger = logger;
        }

        public TppDcrEvents Check(TppClient.TppApiObjectResponse<RequestPatientRecordReply> taskResponse)
        {
            _logger.LogEnter();

            if (taskResponse.HasSuccessResponse)
            {              
                _logger.LogExitWith($"{nameof(taskResponse.HasSuccessResponse)}= true");
                return _dcrEventsMapper.Map(taskResponse.Body);
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

            _logger.LogExit();
            return tppDcrEvents;
        }
    }
}
