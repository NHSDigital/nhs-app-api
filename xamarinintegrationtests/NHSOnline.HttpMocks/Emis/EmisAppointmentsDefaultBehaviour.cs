using System.Collections;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisAppointmentsDefaultBehaviour : IEmisAppointmentsBehaviour
    {
        public IActionResult Behave()
        {
            return new JsonResult(new
            {
                PastAppointments = new ArrayList(),
                UpcomingAppointments = new ArrayList()
            });
        }
    }
}