using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Worker.Support.Date;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsService : IAppointmentsService
    {
        private readonly TppAppointmentsServiceBook _booker;
        private readonly ITppClient _tppClient;
        private readonly ILogger<TppAppointmentsService> _logger;
        private readonly IAppointmentsResultBuilder _appointmentResultBuilder;

        public TppAppointmentsService(
            ITppClient tppClient,
            ILogger<TppAppointmentsService> logger,
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            IAppointmentsResultBuilder appointmentsResultBuilder)
        {
            _booker = new TppAppointmentsServiceBook(logger, tppClient, dateTimeOffsetProvider);
            _logger = logger;
            _tppClient = tppClient;
            _appointmentResultBuilder = appointmentsResultBuilder;
        }

        public async Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request)
        {
            return await _booker.Book((TppUserSession)userSession, request);
        }

        public Task<AppointmentCancelResult> Cancel(UserSession userSession, AppointmentCancelRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<AppointmentsResult> GetAppointments(UserSession userSession, bool includePastAppointments, DateTimeOffset? pastAppointmentsFromDate)
        {
            var tppUserSession = (TppUserSession)userSession;

            try
            {
                var request = new ViewAppointments(tppUserSession);

                var viewAppointmentsTask = _tppClient.ViewAppointmentsPost(request, tppUserSession.Suid);
                await Task.WhenAll(viewAppointmentsTask);

                var result = _appointmentResultBuilder.Build(viewAppointmentsTask);

                return result.ValueOrFailure();

            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "HttpRequestException has been thrown.");
                return new AppointmentsResult.SupplierSystemUnavailable();
            }
        }
    }
}