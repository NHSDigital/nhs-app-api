using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments
{
    public class VisionAppointmentsService : IAppointmentsService
    {
        private readonly ILogger<VisionAppointmentsService> _logger;
        private readonly VisionAppointmentsRetrievalService _getter;
        private readonly VisionAppointmentsBookingService _booker;
        private readonly VisionAppointmentsCancellationService _canceller;
        private readonly IAppointmentCancellationReasonLogger _appointmentCancellationReasonLogger;

        public VisionAppointmentsService(
            ILogger<VisionAppointmentsService> logger,
            VisionAppointmentsRetrievalService getter,
            VisionAppointmentsBookingService booker, 
            VisionAppointmentsCancellationService canceller,
            IAppointmentCancellationReasonLogger appointmentCancellationReasonLogger)
        {
            _logger = logger;
            _getter = getter;
            _booker = booker;
            _canceller = canceller;
            _appointmentCancellationReasonLogger = appointmentCancellationReasonLogger;
        }
        
        public async Task<AppointmentBookResult> Book(GpUserSession gpUserSession, AppointmentBookRequest request)
        {
            return await _booker.Book((VisionUserSession)gpUserSession, request);
        }

        public async Task<AppointmentCancelResult> Cancel(GpUserSession gpUserSession, AppointmentCancelRequest request)
        {
            return await _canceller.Cancel((VisionUserSession)gpUserSession, request);
        }

        public async Task<AppointmentsResult> GetAppointments(GpUserSession gpUserSession)
        {
            var appointmentsResult = await _getter.GetAppointments(gpUserSession);

            try
            {
                _appointmentCancellationReasonLogger.CaptureCancellationReasons(gpUserSession, appointmentsResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to log Vision cancellation reasons.");
            }

            return appointmentsResult;
        }
    }
}
