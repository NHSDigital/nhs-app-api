using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    [FakeGpAreaBehaviour(Behaviour.NoData)]
    public class NoDataAppointmentSlotsAreaBehaviour : IAppointmentSlotsAreaBehaviour
    {
        public Task<AppointmentSlotsResult> GetSlots(
            GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentSlotsDateRange dateRange) =>
            Task.FromResult<AppointmentSlotsResult>(new AppointmentSlotsResult.Success(
                new AppointmentSlotsResponse { Slots = new List<Slot>() }));
    }
}