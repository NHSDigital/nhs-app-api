using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class GetPatientDocumentTaskChecker
    {
        private readonly ILogger<GetPatientDocumentTaskChecker> _logger;
        private readonly EmisPatientDocumentMapper _mapper;
        
        public GetPatientDocumentTaskChecker(ILogger<GetPatientDocumentTaskChecker> logger, EmisPatientDocumentMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }
        
        public PatientDocument Check(Task<EmisClient.EmisApiObjectResponse<IndividualDocument>> task,
            string documentType, string documentName)
        {
            _logger.LogEnter();
            
            PatientDocument document = null;
            
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Retrieving documents task completed unsuccessfully");
                document = new PatientDocument
                {
                    HasErrored = true
                };
            }

            var documentResponse = task.Result;

            if (!documentResponse.HasSuccessResponse)
            {
                _logger.LogError(
                    $"Unsuccessful request retrieving Document for patient. Status code: {(int) documentResponse.StatusCode}");
                document = new PatientDocument
                {
                    HasErrored = true
                };
            }

            if (documentResponse.HasBadRequestResponse)
            {
                _logger.LogError(
                    $"Bad request retrieving Document for patient. The document guid may not be valid. Status code: {(int) documentResponse.StatusCode}");
                document = new PatientDocument
                {
                    Content = documentResponse.StandardErrorResponse.Message,
                    HasErrored = true
                };
            }

            _logger.LogExit();
            return document ?? _mapper.Map(documentResponse.Body, documentType, documentName);
        }      
    }
}
