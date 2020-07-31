using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    [FakeGpAreaBehaviour(Behaviour.Unauthorised)]
    public class UnauthorisedAppointmentsAreaBehaviour : IAppointmentsAreaBehaviour
    {
        public Task<AppointmentBookResult> Book(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentBookRequest request) =>
            throw new UnauthorisedGpSystemHttpRequestException();

        public Task<AppointmentCancelResult> Cancel(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentCancelRequest request) =>
            throw new UnauthorisedGpSystemHttpRequestException();

        public Task<AppointmentsResult> GetAppointments(GpLinkedAccountModel gpLinkedAccountModel) =>
            throw new UnauthorisedGpSystemHttpRequestException();
    }
}