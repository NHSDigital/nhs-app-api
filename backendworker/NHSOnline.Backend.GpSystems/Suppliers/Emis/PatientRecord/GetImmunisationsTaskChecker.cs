using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    internal sealed class GetImmunisationsTaskChecker
    {
        private readonly ILogger<GetImmunisationsTaskChecker> _logger;
        private readonly EmisImmunisationMapper _mapper;
        
        public GetImmunisationsTaskChecker(ILogger<GetImmunisationsTaskChecker> logger, EmisImmunisationMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }
        
        public Immunisations Check(Task<EmisApiObjectResponse<MedicationRootObject>> task)
        {
            _logger.LogEnter();
            
            Immunisations immunisations = null;
            
            if (!task.IsCompletedSuccessfully)
            {
                _logger.LogError("Retrieving immunisations task completed unsuccessfully");
                immunisations = new Immunisations
                {
                    HasErrored = true
                };
            }
            
            var immunisationsResponse = task.Result;
            
            if (!immunisationsResponse.HasSuccessResponse)
            {
                // User does not have access
                if (immunisationsResponse.HasForbiddenResponse() ||
                    immunisationsResponse.HasExceptionWithMessageContaining("Requested record access is disabled by the practice"))
                {
                    _logger.LogWarning("User does not have access to their immunisations in their patient record");
                    immunisations = new Immunisations
                    {
                        HasAccess = false
                    };
                }
                else
                {
                    _logger.LogError(
                        $"Unsuccessful request retrieving immunisations list for patient. Status code: {(int) immunisationsResponse.StatusCode}");
                    immunisations = new Immunisations
                    {
                        HasErrored = true
                    };
                }
            }

            _logger.LogExit();
            return immunisations ?? _mapper.Map(immunisationsResponse.Body);
        }
    }
}
