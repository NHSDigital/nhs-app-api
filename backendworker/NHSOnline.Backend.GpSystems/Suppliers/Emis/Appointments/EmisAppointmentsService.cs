using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments
{
    public class EmisAppointmentsService : IAppointmentsService
    {
        private readonly EmisAppointmentsBookingService _booker;
        private readonly EmisAppointmentsCancellationService _canceller;
        private readonly EmisAppointmentsRetrievalService _getter;

        public EmisAppointmentsService(
            EmisAppointmentsRetrievalService getter,
            EmisAppointmentsBookingService booker,
            EmisAppointmentsCancellationService canceller)
        {
            _getter = getter;
            _booker = booker;
            _canceller = canceller;
        }

        public async Task<AppointmentBookResult> Book(GpLinkedAccountModel gpLinkedAccountModel, AppointmentBookRequest request)
        {
            return await _booker.Book(gpLinkedAccountModel, request);
        }

        public async Task<AppointmentCancelResult> Cancel(GpLinkedAccountModel gpLinkedAccountModel, AppointmentCancelRequest request)
        {
            return await _canceller.Cancel(gpLinkedAccountModel, request);
        }

        public async Task<AppointmentsResult> GetAppointments(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return await _getter.GetAppointments(gpLinkedAccountModel);
        }
    }
}
