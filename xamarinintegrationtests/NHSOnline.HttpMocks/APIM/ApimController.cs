using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.APIM.Models;

namespace NHSOnline.HttpMocks.Controllers.APIM
{
    public class ApimController : Controller
    {
        [HttpPost]
        [Route("oauth2/token")]
        public IActionResult RequestAccessToken()
        {
            return Ok(new TokenResponse
            {
                AccessToken = "accessToken",
                ExpiresIn = "90",
                TokenType = "Bearer",
                IssuedTokenType = "urn:ietf:params:oauth:token-type:access_token"
            });
        }
    }
}
