using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    public class FakeAppointmentsValidationService : AppointmentsValidationService
    {
        public FakeAppointmentsValidationService(ILogger<FakeAppointmentsService> logger) : base(logger)
        {
        }

        protected override bool IsSupplierDeleteValid(AppointmentCancelRequest request) => true;

        protected override bool IsSupplierPostValid(AppointmentBookRequest request) => true;
    }
}