using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Ndop;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Ndop
{
    [Route("patient/ndop"),PfsSecurityMode]
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
        public async Task<IActionResult> GetToken()
        {
            try
            {
                _logger.LogEnter();

                var userSession = HttpContext.GetUserSession();

                await _auditor.Audit(Constants.AuditingTitles.GetNdopTokenAuditTypeRequest, "Getting Ndop JWT Token");

                var result = _ndopService.GetJwtToken(userSession.GpUserSession.NhsNumber);
                result.Accept(new NdopAuditingVisitor(_auditor));

                return result.Accept(new NdopResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}