using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisPatientRecordService : IPatientRecordService
    {
        private readonly ILogger _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisMyRecordMapper _emisMyRecordMapper;

        public EmisPatientRecordService(ILoggerFactory loggerFactory, IEmisClient emisClient, IEmisMyRecordMapper emisMyRecordMapper)
        {
            _emisClient = emisClient;
            _emisMyRecordMapper = emisMyRecordMapper;
            _logger = loggerFactory.CreateLogger<EmisPatientRecordService>();
        }
        
        public async Task<GetMyRecordResult> Get(UserSession userSession)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var allergiesTask = _emisClient.AllergiesGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId);
                
                await Task.WhenAll(allergiesTask);

                if (!allergiesTask.IsCompletedSuccessfully)
                {
                    _logger.LogError("Retrieving allergies task completed unsuccessfully");
                    return new GetMyRecordResult.SuccessfullyRetrieved(new MyRecordResponse
                    {
                        Allergies = new Allergies
                        {
                            HasErrored = true
                        }
                    });  
                }
                
                var allergiesResponse = allergiesTask.Result;
                
                if (!allergiesResponse.HasSuccessStatusCode)
                {
                    // User does not have access
                    if (allergiesResponse.HasExceptionWithMessageContaining("Services Access violation"))
                    {
                        _logger.LogWarning("User does not have access to their patient record");
                        return new GetMyRecordResult.SuccessfullyRetrieved(new MyRecordResponse());
                    }
                    
                    _logger.LogError(
                        $"Unsuccessful request retrieving Allergy list for patient. Status code: {(int) allergiesResponse.StatusCode}");
                    return new GetMyRecordResult.SuccessfullyRetrieved(new MyRecordResponse
                    {
                        Allergies = new Allergies
                        {
                            HasErrored = true
                        }
                    });
                }

                _logger.LogInformation($"Mapping response from {nameof(AllergyRequestsGetResponse)} to {nameof(MyRecordResponse)}");

                var result = _emisMyRecordMapper.Map(allergiesResponse.Body);
                result.Allergies.HasAccess = true;
                
                _logger.LogInformation("MyRecordResponse: " + result);

                return new GetMyRecordResult.SuccessfullyRetrieved(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving my record");
                return new GetMyRecordResult.Unsuccessful();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "My record retrieval return null body");
                return new GetMyRecordResult.SupplierBadData();
            }
        }
    }
}