using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface IGetPatientDocumentTaskChecker
    {
        PatientDocument Check(TppApiObjectResponse<RequestBinaryDataReply> taskResponse);
    }

    public class GetPatientDocumentTaskChecker : IGetPatientDocumentTaskChecker
    {
        private readonly ITppDocumentMapper _documentMapper;
        private readonly ILogger<IGetPatientDocumentTaskChecker> _logger;

        public GetPatientDocumentTaskChecker(ITppDocumentMapper dcrDocumentsMapper, ILogger<GetPatientDocumentTaskChecker> logger)
        {
            _documentMapper = dcrDocumentsMapper;
            _logger = logger;
        }

        public PatientDocument Check(TppApiObjectResponse<RequestBinaryDataReply> taskResponse)
        {
            _logger.LogEnter();

            if (taskResponse.HasSuccessResponse)
            {
                _logger.LogExitWith($"{nameof(taskResponse.HasSuccessResponse)}= true");
                return _documentMapper.Map(taskResponse.Body);
            }

            if (taskResponse.HasForbiddenResponse)
            {
                _logger.LogWarning("User does not have access to request binary data");
            }
            else
            {
                _logger.LogError($"Unsuccessful request retrieving binary data. Status code: {(int)taskResponse.StatusCode}");
            }

            _logger.LogExit();
            return new PatientDocument
            {
                HasErrored = true
            };
        }
    }
}