using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Registrations;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
    public class RegistrationsController : Controller
    {
        private readonly ILogger<RegistrationsController> _logger;
        private readonly IRegistrationService _registrationService;

        public RegistrationsController(IRegistrationService registrationService, ILogger<RegistrationsController> logger)
        {
            _registrationService = registrationService;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/users/{nhsLoginId}/devices/registrations")]
        public async Task<IActionResult> Get(string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                var findRegistrationsResult = await _registrationService.Find(nhsLoginId);

                return findRegistrationsResult.Accept(new FindRegistrationsResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}