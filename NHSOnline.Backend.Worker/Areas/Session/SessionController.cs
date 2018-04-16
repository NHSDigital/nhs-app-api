using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Areas.Session.Models;
using NHSOnline.Backend.Worker.Filters;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    [Route("session")]
    public class SessionController : Controller
    {
        [HttpPost, TimeoutExceptionFilter]
        public async Task<IActionResult> Post([FromBody] UserSessionRequest model)
        {
            var dummyResponse = new UserSessionResponse
            {
                FamilyName = "Doyle",
                GivenName = "James"
            };

            var sessionId = Guid.NewGuid().ToString();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.Now.AddSeconds(1200)   // TODO - NHSO-454 move this into configuration
            };

            Response.Cookies.Append(Cookies.SessionId, sessionId, cookieOptions);

            return await Task.FromResult(new CreatedResult(string.Empty, dummyResponse));
        }
    }
}
