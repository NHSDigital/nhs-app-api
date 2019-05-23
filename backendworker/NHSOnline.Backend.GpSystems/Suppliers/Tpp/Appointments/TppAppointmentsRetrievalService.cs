using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
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

        public async Task<AppointmentsResult> GetAppointments(GpUserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var tppUserSession = (TppUserSession)userSession;
                var requestPast = new ViewAppointments(tppUserSession, false);
                var requestUpcoming = new ViewAppointments(tppUserSession, true);

                var viewPastAppointmentsTask = _tppClient.ViewAppointmentsPost(requestPast, tppUserSession.Suid);
                await viewPastAppointmentsTask;
                
                var viewUpcomingAppointmentsTask = _tppClient.ViewAppointmentsPost(requestUpcoming, tppUserSession.Suid);
                await viewUpcomingAppointmentsTask;
                
                var viewPastAppointments = _appointmentResultBuilder.Build(viewPastAppointmentsTask, viewUpcomingAppointmentsTask);
                return viewPastAppointments.ValueOrFailure();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "HttpRequestException has been thrown.");
                return new AppointmentsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
