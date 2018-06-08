using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisPatientRecordService : IPatientRecordService
    {
        private readonly ILogger _logger;
        private readonly ConfigurationSettings _settings;
        private readonly IEmisClient _emisClient;
        private readonly IEmisAllergyMapper _emisAllergyMapper;

        public EmisPatientRecordService(ILoggerFactory loggerFactory, IOptions<ConfigurationSettings> settings, IEmisClient emisClient, IEmisAllergyMapper emisAllergyMapper)
        {
            _emisClient = emisClient;
            _settings = settings.Value;
            _emisAllergyMapper = emisAllergyMapper;
            _logger = loggerFactory.CreateLogger<EmisPatientRecordService>();
        }
        
        public async Task<GetAllergyResult> GetPatientAllergies(UserSession userSession)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var allergiesResponse = await _emisClient.AllergiesGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId);

                if (!allergiesResponse.HasSuccessStatusCode)
                {
                    // User does not have access
                    if (allergiesResponse.HasExceptionWithMessageContaining("Services Access violation"))
                    {
                        _logger.LogWarning("User does not have access to their patient record");
                        return new GetAllergyResult.UserHasNoAccess();
                    }
                    
                    _logger.LogError(
                        $"Unsuccessful request retrieving Allergy list for patient. Status code: {(int) allergiesResponse.StatusCode}");
                    return new GetAllergyResult.Unsuccessful();
                }

                _logger.LogDebug($"Mapping response from {nameof(AllergyRequestsGetResponse)} to {nameof(AllergyListResponse)}");
                var result = _emisAllergyMapper.Map(allergiesResponse.Body);

                return new GetAllergyResult.SuccessfullyRetrieved(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving Allergy");
                return new GetAllergyResult.Unsuccessful();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "Allergy retrieval return null body");
                return new GetAllergyResult.SupplierBadData();
            }
        }       
    }
}