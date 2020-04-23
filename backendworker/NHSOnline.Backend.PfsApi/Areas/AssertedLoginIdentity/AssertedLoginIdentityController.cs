using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.AssertedLoginIdentity
{
    [ApiVersionRoute("patient/asserted-login-identity")]
    public class AssertedLoginIdentityController : Controller
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<AssertedLoginIdentityController> _logger;
        private readonly IAssertedLoginIdentityService _assertedLoginIdentityService;

        public AssertedLoginIdentityController(IAuditor auditor, ILogger<AssertedLoginIdentityController> logger,
            IAssertedLoginIdentityService assertedLoginIdentityService)
        {
            _auditor = auditor;
            _logger = logger;
            _assertedLoginIdentityService = assertedLoginIdentityService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateJwtRequest model, [UserSession] UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                var result =
                    await userSession.Accept(new AssertedLoginSessionVisitor(model, _auditor,
                        _assertedLoginIdentityService));

                var response = result.Accept(new CreateJwtResultVisitor(_logger, model));

                return response;
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}