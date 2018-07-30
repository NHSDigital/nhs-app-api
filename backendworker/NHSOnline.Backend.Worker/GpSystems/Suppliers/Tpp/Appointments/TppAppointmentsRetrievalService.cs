using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsRetrievalService
    {
        private readonly ILogger<TppAppointmentsRetrievalService> _logger;
        private readonly ITppClient _tppClient;
        private readonly IAppointmentsResultBuilder _appointmentResultBuilder;

        public TppAppointmentsRetrievalService(ILogger<TppAppointmentsRetrievalService> logger, ITppClient tppClient,
            IAppointmentsResultBuilder appointmentResultBuilder)
        {
            _logger = logger;
            _tppClient = tppClient;
            _appointmentResultBuilder = appointmentResultBuilder;
        }

        public async Task<AppointmentsResult> GetAppointments(UserSession userSession, bool includePastAppointments, DateTimeOffset? pastAppointmentsFromDate)
        {
            try
            {
                _logger.LogEnter(nameof(GetAppointments));

                var tppUserSession = (TppUserSession)userSession;
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
            finally
            {
                _logger.LogExit(nameof(GetAppointments));
            }
        }
    }
}
