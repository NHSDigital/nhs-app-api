using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public class EmisAppointmentSlotsResultBuilder
    {
        private readonly ILogger<EmisAppointmentSlotsService> _logger;
        private readonly IAppointmentSlotsResponseMapper _appointmentSlotsResponseMapper;
        private readonly Task<EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> _metaTask;
        private readonly Task<EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>> _slotTask;

        private EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse> MetaResponse => _metaTask.Result;
        private EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse> SlotResponse => _slotTask.Result;

        public EmisAppointmentSlotsResultBuilder(
            ILogger<EmisAppointmentSlotsService> logger,
            IAppointmentSlotsResponseMapper appointmentSlotsResponseMapper,
            Task<EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> metaTask,
            Task<EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>> slotTask
            )
        {
            _logger = logger;
            _appointmentSlotsResponseMapper = appointmentSlotsResponseMapper;
            _metaTask = metaTask;
            _slotTask = slotTask;
        }

        internal Option<AppointmentSlotsResult> Build()
        {
            return GetMetaTaskCompletedUnsuccessfulCase()
                .IfNone(GetMetaResponseNoSuccessStatusCodeCase)
                .IfNone(GetSlotsTaskCompletedUnsuccessfullyCase)
                .IfNone(GetSlotsResponseHasNoSuccessStatusCodeCase)
                .IfNone(BuildSuccessfulAppointmentSlotsResult);
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
            return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.SupplierSystemUnavailable());
        }

        private Option<AppointmentSlotsResult> GetResponseHasNoSuccessStatusCodeCase<T>(T response)
            where T : EmisClient.EmisApiResponse
        {
            if (response.HasSuccessStatusCode)
            {
                return Option.None<AppointmentSlotsResult>();
            }
            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.CannotBookAppointments());
            }
            _logger.LogEmisUnknownError(response);
            return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.SupplierSystemUnavailable());
        }

        private Option<AppointmentSlotsResult> BuildSuccessfulAppointmentSlotsResult()
        {
            try
            {
                var result =
                    new AppointmentSlotsResult.SuccessfullyRetrieved(
                        _appointmentSlotsResponseMapper.Map(SlotResponse.Body, MetaResponse.Body));
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
