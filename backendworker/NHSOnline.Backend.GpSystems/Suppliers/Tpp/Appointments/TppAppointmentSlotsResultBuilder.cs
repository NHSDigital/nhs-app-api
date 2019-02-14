using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support;
using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public interface IAppointmentSlotResultBuilder
    {
        Option<AppointmentSlotsResult> Build(Task<TppClient.TppApiObjectResponse<ListSlotsReply>> slotTask);
    }
    public class TppAppointmentSlotsResultBuilder : IAppointmentSlotResultBuilder
    {
        private readonly ILogger<TppAppointmentSlotsService> _logger;
        private readonly IListSlotsReplyMapper _listSlotsReplyMapper;

        public TppAppointmentSlotsResultBuilder(ILogger<TppAppointmentSlotsService> logger, 
            IListSlotsReplyMapper listSlotsReplyMapper)
        {
            _listSlotsReplyMapper = listSlotsReplyMapper;
            _logger = logger;
        }

        public Option<AppointmentSlotsResult> Build(Task<TppClient.TppApiObjectResponse<ListSlotsReply>> slotTask)
        {
            try
            {
                _logger.LogEnter();
                
                return GetSlotsTaskCompletedUnsuccessfullyCase(slotTask)
                    .IfNone(() => GetSlotsResponseHasNoSuccessStatusCodeCase(slotTask))
                    .IfNone(() => BuildSuccessfulAppointmentSlotsResult(slotTask));
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private Option<AppointmentSlotsResult> BuildSuccessfulAppointmentSlotsResult(Task<TppClient.TppApiObjectResponse<ListSlotsReply>> slotTask)
        {
            try
            {
                var result =
                    new AppointmentSlotsResult.SuccessfullyRetrieved(
                        _listSlotsReplyMapper.Map(slotTask.Result.Body));
                return Option.Some<AppointmentSlotsResult>(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to map successful appointment slot response");
                return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.InternalServerError());
            }
        }
        private Option<AppointmentSlotsResult> GetSlotsResponseHasNoSuccessStatusCodeCase(Task<TppClient.TppApiObjectResponse<ListSlotsReply>> slotTask)
        {
            return GetResponseHasNoSuccessStatusCodeCase(slotTask.Result);
        }

        private Option<AppointmentSlotsResult> GetResponseHasNoSuccessStatusCodeCase<T>(T response)
            where T : TppClient.TppApiObjectResponse<ListSlotsReply>
        {
            if (response.HasSuccessResponse)
            {
                return Option.None<AppointmentSlotsResult>();
            }
            if (response.HasForbiddenResponse)
            {
                _logger.LogTppResponseAccessIsForbidden();
                return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.CannotBookAppointments());
            }
            _logger.LogTppUnknownError(response);
            return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.SupplierSystemUnavailable());
        }

        private Option<AppointmentSlotsResult> GetSlotsTaskCompletedUnsuccessfullyCase(Task<TppClient.TppApiObjectResponse<ListSlotsReply>> slotTask)
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
            return Option.Some<AppointmentSlotsResult>(new AppointmentSlotsResult.SupplierSystemUnavailable());
        }
    }
}
