using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class MicrotestAppointmentsService : IAppointmentsService
    {
        private readonly MicrotestAppointmentsBookingService _booker;
        private readonly MicrotestAppointmentsCancellationService _canceller;
        private readonly MicrotestAppointmentsRetrievalService _getter;
        
        public MicrotestAppointmentsService(
            MicrotestAppointmentsBookingService booker, 
            MicrotestAppointmentsCancellationService canceller,
            MicrotestAppointmentsRetrievalService getter)
        {
            _getter = getter;
            _booker = booker;
            _canceller = canceller;
        }
        
        public async Task<AppointmentBookResult> Book(GpLinkedAccountModel gpLinkedAccountModel, AppointmentBookRequest request)
        {
            return await _booker.Book((MicrotestUserSession) gpLinkedAccountModel.GpUserSession, request);
        }

        public async Task<AppointmentCancelResult> Cancel(GpLinkedAccountModel gpLinkedAccountModel, AppointmentCancelRequest request)
        {
            return await _canceller.Cancel((MicrotestUserSession) gpLinkedAccountModel.GpUserSession, request);
        }

        public async Task<AppointmentsResult> GetAppointments(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return await _getter.GetAppointments((MicrotestUserSession) gpLinkedAccountModel.GpUserSession);
        }
    }
}
