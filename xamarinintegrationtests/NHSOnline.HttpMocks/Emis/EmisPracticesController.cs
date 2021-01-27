using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Emis
{
    [Route("emis/practices/{practice}")]
    public class EmisPracticesController : Controller
    {
        [HttpGet("settings")]
        public IActionResult Appointments(string practice)
        {
            return Json(new
            {
                Messages = new
                {
                    AppointmentsMessage = $@"Welcome to Practice {practice}.
                                            Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed
                                            do eiusmod tempor incididunt ut labore et dolore magna aliqua.
                                            Ut enim ad minim veniam, quis nostrud exercitation ullamco
                                            laboris nisi ut aliquip ex ea commodo consequat. Duis aute
                                            irure dolor inreprehenderit in voluptate velit esse cillum
                                            dolore eu fugiat nulla pariatur. Excepteur sint occaecat
                                            cupidatat non proident, sunt in culpa qui officia deserunt
                                            mollit anim id est laborum"
                },
                Services = new
                {
                    PracticePatientCommunicationSupported = true
                }
            });
        }
    }
}