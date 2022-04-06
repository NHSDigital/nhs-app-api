using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.SecondaryCare;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.SecondaryCare
{
    [ApiVersionRoute("patient/secondary-care")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SecondaryCareController : Controller
    {
        private readonly ILogger<SecondaryCareController> _logger;
        private readonly IAuditor _auditor;
        private readonly ISecondaryCareService _secondaryCareService;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        public SecondaryCareController(
            ILogger<SecondaryCareController> logger,
            IAuditor auditor,
            ISecondaryCareService secondaryCareService,
            IErrorReferenceGenerator errorReferenceGenerator)
        {
            _logger = logger;
            _auditor = auditor;
            _secondaryCareService = secondaryCareService;
            _errorReferenceGenerator = errorReferenceGenerator;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary([UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
                await _auditor.PreOperationAudit(AuditingOperations.SecondaryCareGetSummaryRequest, "Attempting to get Secondary Care Summary");

                var result = await _secondaryCareService.GetSummary(userSession);

                await result.Accept(new SecondaryCareSummaryResultAuditingVisitor(_auditor, _logger));
                return result.Accept(new SecondaryCareSummaryResultVisitor(_errorReferenceGenerator, Supplier.SecondaryCareAggregator));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}