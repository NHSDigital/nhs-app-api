using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support.AspNet;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
    [Route("patient/linked-accounts")]
    public class LinkedAccountsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<LinkedAccountsController> _logger;

        public LinkedAccountsController(
            ILogger<LinkedAccountsController> logger,
            IGpSystemFactory gpSystemFactory)
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userSession = HttpContext.GetUserSession();

            _logger.LogInformation($"Fetching linked accounts supplier {userSession.GpUserSession.Supplier}");
            
            var linkedAccountsService = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetLinkedAccountsService();

            var result = await linkedAccountsService.GetLinkedAccounts(userSession.GpUserSession);
            
            return await result.Accept(new LinkedAccountsResultVisitor());
        }
    }
}
