using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    internal sealed class GetDocumentsTaskChecker
    {
        private readonly ILogger<GetDocumentsTaskChecker> _logger;
        private readonly EmisDocumentsMapper _mapper;
        
        public GetDocumentsTaskChecker(ILogger<GetDocumentsTaskChecker> logger, EmisDocumentsMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }
        
        public PatientDocuments Check(Task<EmisApiObjectResponse<MedicationRootObject>> task)
        {
            _logger.LogEnter();
            
            PatientDocuments documents = null;
            
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Retrieving documents task completed unsuccessfully");
                documents = new PatientDocuments
                {
                    HasErrored = true
                };
            }
            
            var documentResponse = task.Result;
            
            if (!documentResponse.HasSuccessResponse)
            {
                // User does not have access
                if (documentResponse.HasForbiddenResponse() ||
                    documentResponse.HasExceptionWithMessageContaining("Requested record access is disabled by the practice"))
                {
                    _logger.LogWarning("User does not have access to their documents in patient record");
                    documents = new PatientDocuments
                    {
                        HasAccess = false
                    };
                }
                else
                {
                    _logger.LogError(
                        $"Unsuccessful request retrieving Document list for patient. Status code: {(int) documentResponse.StatusCode}");
                    documents = new PatientDocuments
                    {
                        HasErrored = true
                    };
                }
            }
            
            if (documentResponse.Body == null)
            {
                _logger.LogError("Returned document response body is null.");
                
                documents = new PatientDocuments
                {
                    HasErrored = true
                };
            }

            _logger.LogExit();
            return documents ?? _mapper.Map(documentResponse.Body);
            
        }      
    }
}
