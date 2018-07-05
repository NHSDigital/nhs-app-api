using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.Support.Date;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsService : IAppointmentsService
    {
        private readonly TppAppointmentsServiceBook _booker;

        public TppAppointmentsService(
            ITppClient tppClient,
            ILogger<TppAppointmentsService> logger,
            IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _booker = new TppAppointmentsServiceBook(logger, tppClient, dateTimeOffsetProvider);
        }

        public async Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request)
        {
            return await _booker.Book((TppUserSession)userSession, request);
        }

        public Task<AppointmentCancelResult> Cancel(UserSession userSession, AppointmentCancelRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<AppointmentsResult> GetAppointments(UserSession userSession, bool includePastAppointments, DateTimeOffset? pastAppointmentsFromDate)
        {
            throw new NotImplementedException();
        }
    }
}