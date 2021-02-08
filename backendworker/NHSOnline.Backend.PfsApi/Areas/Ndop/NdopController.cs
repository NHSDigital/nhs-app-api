using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.PfsApi.Ndop;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Ndop
{
    [ApiVersionRoute("patient/ndop")]
    [ProxyingNotAllowed]
    public class NdopController : Controller
    {
        private readonly INdopService _ndopService;
        private readonly ILogger<NdopController> _logger;
        private readonly IAuditor _auditor;

        public NdopController(
            ILogger<NdopController> logger,
            INdopService ndopService,
            IAuditor auditor)
        {
            _ndopService = ndopService;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpGet]
        public async Task<IActionResult> GetToken([UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                await _auditor.PreOperationAudit(AuditingOperations.GetNdopTokenAuditTypeRequest, "Getting Ndop JWT Token");

                var result = _ndopService.GetJwtToken(userSession.NhsNumber);

                await result.Accept(new NdopAuditingVisitor(_auditor, _logger));
                return result.Accept(new NdopResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
