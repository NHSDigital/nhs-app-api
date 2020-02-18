using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    internal class TppAppointmentsRetrievalService
    {
        private readonly ILogger<TppAppointmentsRetrievalService> _logger;
        private readonly ITppClientRequest<(ViewAppointments viewAppointments, string suid), ViewAppointmentsReply> _viewAppointments;
        private readonly IAppointmentsResultBuilder _appointmentResultBuilder;

        public TppAppointmentsRetrievalService(
            ILogger<TppAppointmentsRetrievalService> logger,
            ITppClientRequest<(ViewAppointments viewAppointments, string suid), ViewAppointmentsReply> viewAppointments,
            IAppointmentsResultBuilder appointmentResultBuilder)
        {
            _logger = logger;
            _viewAppointments = viewAppointments;
            _appointmentResultBuilder = appointmentResultBuilder;
        }

        public async Task<AppointmentsResult> GetAppointments(GpLinkedAccountModel gpLinkedAccountModel)
        {
            try
            {
                _logger.LogEnter();

                var tppUserSession = (TppUserSession)gpLinkedAccountModel.GpUserSession;
                var requestPast = new ViewAppointments(tppUserSession, false);
                var requestUpcoming = new ViewAppointments(tppUserSession, true);

                var viewPastAppointmentsTask = _viewAppointments.Post((requestPast, tppUserSession.Suid));
                await viewPastAppointmentsTask;
                
                var viewUpcomingAppointmentsTask = _viewAppointments.Post((requestUpcoming, tppUserSession.Suid));
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
