using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsService : IAppointmentsService
    {
        private readonly TppAppointmentsRetrievalService _getter;
        private readonly TppAppointmentsBookingService _booker;
        private readonly TppAppointmentsCancellationService _canceller;

        public TppAppointmentsService(
            TppAppointmentsRetrievalService getter,
            TppAppointmentsBookingService booker,
            TppAppointmentsCancellationService canceller)
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
