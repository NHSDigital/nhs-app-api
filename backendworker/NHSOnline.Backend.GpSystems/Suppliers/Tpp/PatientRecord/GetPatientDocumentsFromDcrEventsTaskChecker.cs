using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface IGetPatientDocumentsFromDcrEventsTaskChecker
    {
        PatientDocuments Check(TppApiObjectResponse<RequestPatientRecordReply> taskResponse);
    }

    internal sealed class GetPatientDocumentsFromDcrEventsTaskChecker : IGetPatientDocumentsFromDcrEventsTaskChecker
    {
        private readonly ITppDcrEventsDocumentsMapper _dcrDocumentsMapper;
        private readonly ILogger<IGetPatientDocumentsFromDcrEventsTaskChecker> _logger;

        public GetPatientDocumentsFromDcrEventsTaskChecker(ITppDcrEventsDocumentsMapper dcrDocumentsMapper, ILogger<GetPatientDocumentsFromDcrEventsTaskChecker> logger)
        {
            _dcrDocumentsMapper = dcrDocumentsMapper;
            _logger = logger;
        }

        public PatientDocuments Check(TppApiObjectResponse<RequestPatientRecordReply> taskResponse)
        {
            _logger.LogEnter();

            if (taskResponse.HasSuccessResponse)
            {
                _logger.LogExitWith($"{nameof(taskResponse.HasSuccessResponse)}= true");
                return _dcrDocumentsMapper.Map(taskResponse.Body);
            }

            PatientDocuments tppPatientDocuments;

            if (taskResponse.HasForbiddenResponse)
            {
                _logger.LogWarning("User does not have access to their patient record for Tpp for patientDocuments");
                tppPatientDocuments = new PatientDocuments
                {
                    HasAccess = false
                };
            }
            else
            {
                _logger.LogError($"Unsuccessful request retrieving patient record information for Tpp for patientDocuments. Status code: {(int)taskResponse.StatusCode}");
                tppPatientDocuments = new PatientDocuments
                {
                    HasErrored = true
                };
            }

            _logger.LogExit();
            return tppPatientDocuments;
        }
    }
}
