using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Metrics;
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
        private readonly IMetricLogger _metricLogger;
        private readonly IAssertedLoginIdentityService _assertedLoginIdentityService;

        public AssertedLoginIdentityController(
            IAuditor auditor,
            ILogger<AssertedLoginIdentityController> logger,
            IMetricLogger metricLogger,
            IAssertedLoginIdentityService assertedLoginIdentityService)
        {
            _auditor = auditor;
            _logger = logger;
            _metricLogger = metricLogger;
            _assertedLoginIdentityService = assertedLoginIdentityService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateJwtRequest model, [UserSession] P5UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                var assertedLoginSessionVisitor = new AssertedLoginSessionVisitor(model, _auditor, _assertedLoginIdentityService);
                var result = await userSession.Accept(assertedLoginSessionVisitor);

                return await result.Accept(new CreateJwtResultVisitor(_logger, _metricLogger, model, userSession));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}