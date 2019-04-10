using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class MicrotestAppointmentsCancellationService
    {
        private readonly ILogger<MicrotestAppointmentsBookingService> _logger;
        private readonly IMicrotestClient _microtestClient;
        private readonly ICancellationReasonService _cancellationReasonService;

        public MicrotestAppointmentsCancellationService(
            ILogger<MicrotestAppointmentsBookingService> logger,
            IMicrotestClient microtestClient,
            ICancellationReasonService cancellationReasonService)
        {
            _logger = logger;
            _microtestClient = microtestClient;
            _cancellationReasonService = cancellationReasonService;
        }

        public async Task<AppointmentCancelResult> Cancel(MicrotestUserSession userSession,
            AppointmentCancelRequest request)
        {
            try
            {
                _logger.LogEnter();
                var deleteRequestOption = GetCancelAppointmentDeleteRequest(request);
                if (deleteRequestOption.IsEmpty)
                {
                    return new AppointmentCancelResult.BadRequest();
                }

                var deleteRequest = deleteRequestOption.ValueOrFailure();
                var response = await _microtestClient.AppointmentsDelete(userSession.OdsCode, userSession.NhsNumber, deleteRequest);
                return InterpretAppointmentsDeleteResponse(response);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Cancelling an appointment failed.");
                return new AppointmentCancelResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private AppointmentCancelResult InterpretAppointmentsDeleteResponse(
            MicrotestClient.MicrotestApiObjectResponse<string> response)
        {
            if (response.HasSuccessResponse)
            {
                return new AppointmentCancelResult.SuccessfullyCancelled();
            }

            _logger.LogError(response.ErrorForLogging);
            return new AppointmentCancelResult.SupplierSystemUnavailable();
        }

        private Option<CancelAppointmentDeleteRequest> GetCancelAppointmentDeleteRequest(
            AppointmentCancelRequest request
        )
        {
            if (_cancellationReasonService.TryGetCancellationReason(request.CancellationReasonId,
                out var cancellationReason))
            {
                var deleteRequest =
                    new CancelAppointmentDeleteRequest(request.AppointmentId, cancellationReason.DisplayName);

                return Option.Some(deleteRequest);
            }

            return Option.None<CancelAppointmentDeleteRequest>();
        }
    }
}