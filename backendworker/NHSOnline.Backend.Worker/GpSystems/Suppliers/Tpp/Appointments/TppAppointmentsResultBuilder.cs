using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Worker.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public interface IAppointmentsResultBuilder
    {
        Option<AppointmentsResult> Build(Task<TppClient.TppApiObjectResponse<ViewAppointmentsReply>> appointmentTask);
    }
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

        public Option<AppointmentsResult> Build(Task<TppClient.TppApiObjectResponse<ViewAppointmentsReply>> appointmentTask)
        {
            return GetTaskCompletedUnsuccessfullyCase(appointmentTask)
               .IfNone(() => GetResponseHasNoSuccessStatusCodeCase(appointmentTask.Result))
               .IfNone(() => BuildSuccessfulAppointmentsResult(appointmentTask));
        }

        private Option<AppointmentsResult> BuildSuccessfulAppointmentsResult(Task<TppClient.TppApiObjectResponse<ViewAppointmentsReply>> appointmentTask)
        {
            try
            {
                var result =
                    new AppointmentsResult.SuccessfullyRetrieved(
                        _appointmentsReplyMapper.Map(appointmentTask.Result.Body));
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
                return Option.Some<AppointmentsResult>(new AppointmentsResult.CannotViewAppointments());
            }
            _logger.LogTppUnknownError(response);
            return Option.Some<AppointmentsResult>(new AppointmentsResult.SupplierSystemUnavailable());
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
            return Option.Some<AppointmentsResult>(new AppointmentsResult.SupplierSystemUnavailable());
        }
    }
}
