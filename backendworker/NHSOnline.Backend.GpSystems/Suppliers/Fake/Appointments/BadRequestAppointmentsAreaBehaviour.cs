using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    [FakeGpAreaBehaviour(Behaviour.BadRequest)]
    public class BadRequestAppointmentsAreaBehaviour : IAppointmentsAreaBehaviour
    {
        public Task<AppointmentBookResult> Book(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentBookRequest request) =>
            Task.FromResult<AppointmentBookResult>(new AppointmentBookResult.BadRequest());

        public Task<AppointmentCancelResult> Cancel(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentCancelRequest request) =>
            Task.FromResult<AppointmentCancelResult>(new AppointmentCancelResult.BadRequest());

        public Task<AppointmentsResult> GetAppointments(GpLinkedAccountModel gpLinkedAccountModel) =>
            Task.FromResult<AppointmentsResult>(new AppointmentsResult.BadRequest());
    }
}