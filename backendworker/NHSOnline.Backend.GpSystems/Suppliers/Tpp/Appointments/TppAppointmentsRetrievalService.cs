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
        private readonly ITppClientRequest<(TppRequestParameters, AppointmentViewType), ViewAppointmentsReply> _viewAppointments;
        private readonly IAppointmentsResultBuilder _appointmentResultBuilder;

        public TppAppointmentsRetrievalService(
            ILogger<TppAppointmentsRetrievalService> logger,
            ITppClientRequest<(TppRequestParameters, AppointmentViewType), ViewAppointmentsReply> viewAppointments,
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
                var tppRequestParams = gpLinkedAccountModel.BuildTppRequestParameters(_logger);

                var viewPastAppointmentsTask = _viewAppointments.Post((tppRequestParams, AppointmentViewType.Past));
                await viewPastAppointmentsTask;

                var viewUpcomingAppointmentsTask = _viewAppointments.Post((tppRequestParams, AppointmentViewType.Future));
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
