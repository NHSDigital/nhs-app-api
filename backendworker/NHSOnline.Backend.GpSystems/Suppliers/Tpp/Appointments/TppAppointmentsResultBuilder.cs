using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support;
using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsResultBuilder : IAppointmentsResultBuilder
    {
        private readonly IAppointmentsReplyMapper _appointmentsReplyMapper;
        private readonly ILogger<TppAppointmentsService> _logger;

        public TppAppointmentsResultBuilder(ILogger<TppAppointmentsService> logger,
            IAppointmentsReplyMapper appointmentsReplyMapper)
        {
            _appointmentsReplyMapper = appointmentsReplyMapper;
            _logger = logger;
        }

        public Option<AppointmentsResult> Build(Task<TppClient.TppApiObjectResponse<ViewAppointmentsReply>> viewPastAppointmentsTask, Task<TppClient.TppApiObjectResponse<ViewAppointmentsReply>> viewUpcomingAppointmentsTask)
        {
            return GetTaskCompletedUnsuccessfullyCase(viewPastAppointmentsTask)
                .IfNone(() => GetTaskCompletedUnsuccessfullyCase(viewUpcomingAppointmentsTask))
                .IfNone(() => GetResponseHasNoSuccessStatusCodeCase(viewPastAppointmentsTask.Result))
                .IfNone(() => GetResponseHasNoSuccessStatusCodeCase(viewUpcomingAppointmentsTask.Result))
                .IfNone(() => BuildSuccessfulAppointmentsResult(viewPastAppointmentsTask, viewUpcomingAppointmentsTask));
        }

        private Option<AppointmentsResult> BuildSuccessfulAppointmentsResult(Task<TppClient.TppApiObjectResponse<ViewAppointmentsReply>> viewPastAppointmentsTask, Task<TppClient.TppApiObjectResponse<ViewAppointmentsReply>> viewUpcomingAppointmentsTask)
        {
            try
            {
                var appointmentsResponse = _appointmentsReplyMapper.Map(viewPastAppointmentsTask.Result.Body, viewUpcomingAppointmentsTask.Result.Body);
                var result = new AppointmentsResult.Success(appointmentsResponse);
                return Option.Some<AppointmentsResult>(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to map successful appointment slot response");
                return Option.Some<AppointmentsResult>(new AppointmentsResult.InternalServerError());
            }
        }

        private Option<AppointmentsResult> GetResponseHasNoSuccessStatusCodeCase<T>(T response)
            where T : TppClient.TppApiObjectResponse<ViewAppointmentsReply>
        {
            if (response.HasSuccessResponse)
            {
                return Option.None<AppointmentsResult>();
            }
            if (response.HasForbiddenResponse)
            {
                _logger.LogTppResponseAccessIsForbidden();
                return Option.Some<AppointmentsResult>(new AppointmentsResult.Forbidden());
            }
            _logger.LogTppUnknownError(response);
            return Option.Some<AppointmentsResult>(new AppointmentsResult.BadGateway());
        }

        private Option<AppointmentsResult> GetTaskCompletedUnsuccessfullyCase(Task<TppClient.TppApiObjectResponse<ViewAppointmentsReply>> appointmentTask)
        {
            return GetTaskIsCompletedUnsuccessfullyCase(appointmentTask.IsCompletedSuccessfully,
                "Retrieving view appointments task completed unsuccessfully");
        }

        private Option<AppointmentsResult> GetTaskIsCompletedUnsuccessfullyCase(
            bool completedSuccessfully,
            string message)
        {
            if (completedSuccessfully)
            {
                return Option.None<AppointmentsResult>();
            }
            _logger.LogError(message);
            return Option.Some<AppointmentsResult>(new AppointmentsResult.BadGateway());
        }
    }
}
