using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments
{
    public class EmisAppointmentsValidationService : AppointmentsValidationService
    {
        public EmisAppointmentsValidationService(ILogger<EmisAppointmentsValidationService> logger) : base(logger)
        {
        }

        protected override bool IsSupplierDeleteValid(AppointmentCancelRequest request) => true;
        protected override bool IsSupplierPostValid(AppointmentBookRequest request) => true;
    }
}
