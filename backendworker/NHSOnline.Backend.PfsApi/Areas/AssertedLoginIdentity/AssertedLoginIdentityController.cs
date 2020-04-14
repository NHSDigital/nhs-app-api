using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

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
        public async Task<IActionResult> Post([FromBody] CreateJwtRequest model, [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                await _auditor.Audit(AuditingOperations.CreateAssertedLoginIdentityTokenRequest,
                    "Creating Asserted login Identity JWT for intended relying party URL: " + model.IntendedRelyingPartyUrl);

                var result = _assertedLoginIdentityService.CreateJwtToken(userSession.CitizenIdUserSession.IdTokenJti);

                await result.Accept(new CreateJwtAuditingVisitor(_auditor, _logger, model.IntendedRelyingPartyUrl));
                return result.Accept(new CreateJwtResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}