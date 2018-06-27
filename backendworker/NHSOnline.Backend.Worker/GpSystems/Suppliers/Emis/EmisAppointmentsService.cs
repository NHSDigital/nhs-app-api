using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisAppointmentsService : IAppointmentsService
    {
        private readonly EmisAppointmentsServiceBook _booker;
        private readonly EmisAppointmentsServiceCancel _canceller;
        private readonly EmisAppointmentsServiceGetAppointments _getter;

        public EmisAppointmentsService(
            IEmisClient emisClient, 
            IAppointmentsResponseMapper responseMapper,
            ILogger<EmisAppointmentsService> logger)
        {
            _getter = new EmisAppointmentsServiceGetAppointments(logger, emisClient, responseMapper);
            _booker = new EmisAppointmentsServiceBook(logger, emisClient);
            _canceller = new EmisAppointmentsServiceCancel(logger, emisClient);
        }

        public async Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request)
        {
            return await _booker.Book((EmisUserSession) userSession, request);
        }

        public async Task<AppointmentCancelResult> Cancel(UserSession userSession, AppointmentCancelRequest request)
        {
            return await _canceller.Cancel((EmisUserSession) userSession, request);
        }

        public async Task<AppointmentsResult> GetAppointments(UserSession userSession, 
            bool includePastAppointments,
            DateTimeOffset? pastAppointmentsFromDate)
        {
            return await _getter.GetAppointments((EmisUserSession) userSession,
                includePastAppointments,
                pastAppointmentsFromDate);
        }
    }
}