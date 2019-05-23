using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments
{
    public class EmisAppointmentSlotsResultBuilder
    {
        private readonly ILogger<EmisAppointmentSlotsService> _logger;
        private readonly IAppointmentSlotsResponseMapper _appointmentSlotsResponseMapper;
        private readonly Task<EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> _metaTask;
        private readonly Task<EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>> _slotTask;
        private readonly Task<EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>> _practiceSettingsTask;
        private readonly Task<EmisClient.EmisApiObjectResponse<DemographicsGetResponse>> _demographicsTask;
        private readonly EmisUserSession _userSession;

        private EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse> MetaResponse => _metaTask.Result;
        private EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse> SlotResponse => _slotTask.Result;
        private EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse> PracticeSettingsResponse
            => _practiceSettingsTask.Status == TaskStatus.RanToCompletion ? _practiceSettingsTask.Result : null;
        private EmisClient.EmisApiObjectResponse<DemographicsGetResponse> DemographicsResponse
            => _demographicsTask.Status == TaskStatus.RanToCompletion ? _demographicsTask.Result : null;

        public EmisAppointmentSlotsResultBuilder(
            ILogger<EmisAppointmentSlotsService> logger,
            IAppointmentSlotsResponseMapper appointmentSlotsResponseMapper,
            Task<EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> metaTask,
            Task<EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>> slotTask,
            Task<EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>> practiceSettingsTask,
            Task<EmisClient.EmisApiObjectResponse<DemographicsGetResponse>> demographicsTask,
            EmisUserSession userSession
            )
        {
            _logger = logger;
            _appointmentSlotsResponseMapper = appointmentSlotsResponseMapper;
            _metaTask = metaTask;
            _slotTask = slotTask;
            _practiceSettingsTask = practiceSettingsTask;
            _demographicsTask = demographicsTask;
            _userSession = userSession;
        }

        internal Option<AppointmentSlotsResult> Build()
        {
            try
            {
                _logger.LogEnter();

                return GetMetaTaskCompletedUnsuccessfulCase()
                    .IfNone(GetMetaResponseNoSuccessStatusCodeCase)
                    .IfNone(GetSlotsTaskCompletedUnsuccessfullyCase)
                    .IfNone(GetSlotsResponseHasNoSuccessStatusCodeCase)
                    .IfNone(BuildSuccessfulAppointmentSlotsResult);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private Option<AppointmentSlotsResult> GetMetaTaskCompletedUnsuccessfulCase()
        {
            return GetTaskIsCompletedUnsuccessfullyCase(_metaTask.IsCompletedSuccessfully,
                "Retrieving appointment slots metadata task completed unsuccessfully");
        }

        private Option<AppointmentSlotsResult> GetMetaResponseNoSuccessStatusCodeCase()
        {
            return GetResponseHasNoSuccessStatusCodeCase(MetaResponse);
        }

        private Option<AppointmentSlotsResult> GetSlotsTaskCompletedUnsuccessfullyCase()
        {
            return GetTaskIsCompletedUnsuccessfullyCase(_slotTask.IsCompletedSuccessfully,
                "Retrieving appointment slots task completed unsuccessfully");
        }
        
        private Option<AppointmentSlotsResult> GetSlotsResponseHasNoSuccessStatusCodeCase()
        {
            return GetResponseHasNoSuccessStatusCodeCase(SlotResponse);
        }

        private Option<AppointmentSlotsResult> GetTaskIsCompletedUnsuccessfullyCase(
            bool completedSuccessfully, 
            string message)
        {
            if (completedSuccessfully)
            {
                return Option.None<AppointmentSlotsResult>();
            }
            _logger.LogError(message);
            return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.BadGateway());
        }

        private Option<AppointmentSlotsResult> GetResponseHasNoSuccessStatusCodeCase<T>(T response)
            where T : EmisClient.EmisApiResponse
        {
            if (response.HasSuccessResponse)
            {
                return Option.None<AppointmentSlotsResult>();
            }
            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(response);
                return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.Forbidden());
            }
            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);
            return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.BadGateway());
        }

        private Option<AppointmentSlotsResult> BuildSuccessfulAppointmentSlotsResult()
        {
            try
            {
                var result =
                    new AppointmentSlotsResult.Success(
                        _appointmentSlotsResponseMapper.Map(
                            SlotResponse.Body,
                            MetaResponse.Body,
                            PracticeSettingsResponse?.Body,
                            DemographicsResponse?.Body,
                            _userSession));

                return Option.Some<AppointmentSlotsResult>(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to map successful appointment slot response");
                return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.InternalServerError());
            }
        }
    }
}
