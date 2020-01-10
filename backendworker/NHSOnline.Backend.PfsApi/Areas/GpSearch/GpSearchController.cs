using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Areas.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.GpSearch
{
    [ApiVersionRoute("gpsearch")]
    public class GpSearchController : Controller
    {
        private readonly IGpSearchService _gpSearchService;
        private readonly ILogger<GpSearchController> _logger;
        
        public GpSearchController(
            ILoggerFactory loggerFactory,
            IGpSearchService gpSearchService)
        {
            _gpSearchService = gpSearchService;
            _logger = loggerFactory.CreateLogger<GpSearchController>();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SearchGpPractices([FromBody]GpSearchRequest searchInput)
        {   
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }
                
                _logger.LogInformation($"Fetching Gp Practices for {searchInput.SearchTerm}");
                
                var result = await _gpSearchService.Search(searchInput.SearchTerm);
               
                return result.Accept(new GpSearchResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}