using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
﻿using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public class VisionAppointmentsService : IAppointmentsService
    {
        private readonly VisionAppointmentsRetrievalService _getter;
        private readonly VisionAppointmentsBookingService _booker;
        private readonly VisionAppointmentsCancellationService _canceller;

        public VisionAppointmentsService(
            VisionAppointmentsRetrievalService getter,
            VisionAppointmentsBookingService booker, 
            VisionAppointmentsCancellationService canceller)
        {
            _getter = getter;
            _booker = booker;
            _canceller = canceller;
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
            return await _getter.GetAppointments(gpUserSession);
        }
    }
}
