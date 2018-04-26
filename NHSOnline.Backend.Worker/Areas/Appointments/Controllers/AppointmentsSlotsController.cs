using System;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Controllers
{
    [Route("patient/appointment-slots")]
    public class AppointmentsSlotsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            Clinician[] clinicians =
            {
                new Clinician { Id = Guid.NewGuid().ToString(), DisplayName = "Dr Smith" },
                new Clinician { Id = Guid.NewGuid().ToString(), DisplayName = "Nurse Jones"},
                new Clinician { Id = Guid.NewGuid().ToString(), DisplayName = "Ms Brown"},
            };

            Location[] locations =
            {
                new Location { Id = Guid.NewGuid().ToString(), DisplayName = "Main Surgery" },
                new Location { Id = Guid.NewGuid().ToString(), DisplayName = "Small Clinic" }
            };

            AppointmentSession[] appointmentSessions = {
                new AppointmentSession { Id = Guid.NewGuid().ToString(), DisplayName = "Foot Clinic" },
                new AppointmentSession { Id = Guid.NewGuid().ToString(), DisplayName = "Eye Clinic" },
                new AppointmentSession { Id = Guid.NewGuid().ToString(), DisplayName = "Ear Clinic" }
            };

            Slot[] slots =
            {
                new Slot
                {
                    AppointmentSessionId = appointmentSessions[0].Id,
                    ClinicianIds = new[] { clinicians[0].Id },
                    StartTime = DateTime.Now.AddDays(1),
                    EndTime = DateTime.Now.AddDays(1).AddMinutes(15),
                    Id = Guid.NewGuid().ToString(),
                    LocationId = locations[0].Id
                },
                new Slot
                {
                    AppointmentSessionId = appointmentSessions[1].Id,
                    ClinicianIds = new[] { clinicians[1].Id, clinicians[2].Id },
                    StartTime = DateTime.Now.AddDays(1).AddMinutes(30),
                    EndTime = DateTime.Now.AddDays(1).AddMinutes(45),
                    Id = Guid.NewGuid().ToString(),
                    LocationId = locations[1].Id
                },
                new Slot
                {
                    AppointmentSessionId = appointmentSessions[0].Id,
                    ClinicianIds = new[] { clinicians[1].Id },
                    StartTime = DateTime.Now.AddDays(1).AddMinutes(30),
                    EndTime = DateTime.Now.AddDays(1).AddMinutes(45),
                    Id = Guid.NewGuid().ToString(),
                    LocationId = locations[0].Id
                },
                new Slot
                {
                    AppointmentSessionId = appointmentSessions[1].Id,
                    ClinicianIds = new[] { clinicians[2].Id },
                    StartTime = DateTime.Now.AddDays(2).AddMinutes(30),
                    EndTime = DateTime.Now.AddDays(2).AddMinutes(45),
                    Id = Guid.NewGuid().ToString(),
                    LocationId = locations[0].Id
                },
                new Slot
                {
                    AppointmentSessionId = appointmentSessions[1].Id,
                    ClinicianIds = new[] { clinicians[1].Id, clinicians[0].Id, clinicians[2].Id },
                    StartTime = DateTime.Now.AddDays(3).AddMinutes(30),
                    EndTime = DateTime.Now.AddDays(3).AddMinutes(60),
                    Id = Guid.NewGuid().ToString(),
                    LocationId = locations[1].Id
                }

            };

            var response = new AppointmentSlotsResponse
            {
                AppointmentSessions = appointmentSessions,
                Clinicians = clinicians,
                Locations = locations,
                Slots = slots
            };

            return Json(response);
        }
    }
}
