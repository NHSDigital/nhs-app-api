using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    [FakeGpAreaBehaviour(Behaviour.Default)]
    public class DefaultAppointmentSlotsAreaBehaviour : IAppointmentSlotsAreaBehaviour
    {
        public Task<AppointmentSlotsResult> GetSlots(
            GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentSlotsDateRange dateRange) =>
            Task.FromResult<AppointmentSlotsResult>(new AppointmentSlotsResult.Success(GetFakeSlots()));

        private static AppointmentSlotsResponse GetFakeSlots()
        {
            return new AppointmentSlotsResponse
            {
                Slots = new List<Slot>
                {
                    new Slot
                    {
                        Id = Guid.NewGuid().ToString(),
                        Location = "Fake Place Surgery",
                        StartTime = DateTimeOffset.Now.AddDays(3).AddMinutes(-15),
                        EndTime = DateTimeOffset.Now.AddDays(3),
                        Clinicians = new [] { "Dr Who" },
                        Channel = Channel.Unknown,
                        Type = "GP",
                        TypeFromGpSystem = "Gp_Appt",
                        SessionName = "GP Checkup"
                    },
                    new Slot
                    {
                        Id = Guid.NewGuid().ToString(),
                        Location = "Fake Street Hospital",
                        StartTime = DateTimeOffset.Now.AddDays(3).AddMinutes(-15),
                        EndTime = DateTimeOffset.Now.AddDays(3),
                        Clinicians = new [] { "Dr Dolittle" },
                        Channel = Channel.Telephone,
                        Type = "Outpatient",
                        TypeFromGpSystem = "Gp_Outpatient_Appt",
                        SessionName = "Hospital Tests"
                    }
                }
            };
        }
    }
}