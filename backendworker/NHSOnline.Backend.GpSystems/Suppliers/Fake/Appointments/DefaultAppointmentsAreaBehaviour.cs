using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    [FakeGpAreaBehaviour(Behaviour.Default)]
    public class DefaultAppointmentsAreaBehaviour : IAppointmentsAreaBehaviour
    {
        public Task<AppointmentBookResult> Book(
            GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentBookRequest request) =>
            Task.FromResult<AppointmentBookResult>(new AppointmentBookResult.Success());

        public Task<AppointmentCancelResult> Cancel(
            GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentCancelRequest request) =>
             Task.FromResult<AppointmentCancelResult>(new AppointmentCancelResult.Success());

        public Task<AppointmentsResult> GetAppointments(GpLinkedAccountModel gpLinkedAccountModel) =>
            Task.FromResult<AppointmentsResult>(new AppointmentsResult.Success(GetFakeAppointmentsResponse()));

        private AppointmentsResponse GetFakeAppointmentsResponse()
        {
            return new AppointmentsResponse()
            {
                PastAppointments = new List<PastAppointment>
                {
                    new PastAppointment
                    {
                        Channel = Channel.Unknown,
                        Clinicians = new [] { "Dr Strangelove" },
                        Id = Guid.NewGuid().ToString(),
                        Location = "Outer Space",
                        SessionName = "Medication Review",
                        StartTime = DateTimeOffset.Now.AddDays(-14).AddMinutes(-15),
                        EndTime = DateTimeOffset.Now.AddDays(-14),
                        TelephoneNumber = "028(90) 960 194",
                        Type ="Outpatient",
                        TypeFromGpSystem = "Gp_Outpatient_Appt"
                    }
                },
                UpcomingAppointments = new List<UpcomingAppointment>
                {
                    new UpcomingAppointment
                    {
                        Channel = Channel.Telephone,
                        Clinicians = new [] { "Dr Neo Cortex" },
                        Id = Guid.NewGuid().ToString(),
                        Location = "Wumpa Islands",
                        SessionName = "Minor Surgery",
                        StartTime = DateTimeOffset.Now.AddDays(7).AddMinutes(-30),
                        EndTime = DateTimeOffset.Now.AddDays(7),
                        TelephoneNumber = "028(37) 960 194",
                        Type ="Outpatient Surgery",
                        TypeFromGpSystem = "Gp_Outpatient_Surgery_Appt"
                    }
                },
                CancellationReasons = new[] { new CancellationReason() { Id = "Cancel1", DisplayName = "Not needed" } },
                DisableCancellation = true,
                PastAppointmentsEnabled = true
            };
        }
    }
}