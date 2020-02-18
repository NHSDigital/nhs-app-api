using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support;
using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    internal class TppAppointmentSlotsResultBuilder
    {
        private readonly ILogger<TppAppointmentSlotsService> _logger;
        private readonly IAppointmentSlotsMapper _appointmentSlotsMapper;

        public TppAppointmentSlotsResultBuilder(
            ILogger<TppAppointmentSlotsService> logger, 
            IAppointmentSlotsMapper appointmentSlotsMapper)
        {
            _appointmentSlotsMapper = appointmentSlotsMapper;
            _logger = logger;
        }

        public Option<AppointmentSlotsResult> Build(
            Task<TppApiObjectResponse<ListSlotsReply>> slotTask,
            Task<TppApiObjectResponse<RequestSystmOnlineMessagesReply>> messagesTask)
        {
            try
            {
                _logger.LogEnter();
                
                return GetSlotsTaskCompletedUnsuccessfullyCase(slotTask)
                    .IfNone(() => GetSlotsResponseHasNoSuccessStatusCodeCase(slotTask))
                    .IfNone(() => BuildSuccessfulAppointmentSlotsResult(slotTask, messagesTask));
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private Option<AppointmentSlotsResult> BuildSuccessfulAppointmentSlotsResult(
            Task<TppApiObjectResponse<ListSlotsReply>> slotTask,
            Task<TppApiObjectResponse<RequestSystmOnlineMessagesReply>> messagesTask)
        {
            try
            {
                var messages = messagesTask.Status == TaskStatus.RanToCompletion
                    ? messagesTask.Result?.Body
                    : null;

                var response = _appointmentSlotsMapper.Map(slotTask.Result.Body, messages);
                
                var result = new AppointmentSlotsResult.Success(response);
                
                return Option.Some<AppointmentSlotsResult>(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to map successful appointment slot response");
                return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.InternalServerError());
            }
        }
        private Option<AppointmentSlotsResult> GetSlotsResponseHasNoSuccessStatusCodeCase(Task<TppApiObjectResponse<ListSlotsReply>> slotTask)
        {
            return GetResponseHasNoSuccessStatusCodeCase(slotTask.Result);
        }

        private Option<AppointmentSlotsResult> GetResponseHasNoSuccessStatusCodeCase<T>(T response)
            where T : TppApiObjectResponse<ListSlotsReply>
        {
            if (response.HasSuccessResponse)
            {
                return Option.None<AppointmentSlotsResult>();
            }
            if (response.HasForbiddenResponse)
            {
                _logger.LogTppResponseAccessIsForbidden();
                return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.Forbidden());
            }
            _logger.LogTppUnknownError(response);
            return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.BadGateway());
        }

        private Option<AppointmentSlotsResult> GetSlotsTaskCompletedUnsuccessfullyCase(Task<TppApiObjectResponse<ListSlotsReply>> slotTask)
        {
            return GetTaskIsCompletedUnsuccessfullyCase(slotTask.IsCompletedSuccessfully,
                "Retrieving appointment slots task completed unsuccessfully");
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
    }
}
