using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Brothermailer.Models;
using NHSOnline.Backend.Worker.Brothermailer;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Brothermailer
{
    [Route("brothermailer"),PfsSecurityMode]
    public class BrothermailerController : Controller
    {
        private readonly IBrothermailerService _brothermailerService;
        private readonly ILogger<BrothermailerController> _logger;
        
        public BrothermailerController(
            ILoggerFactory loggerFactory,
            IBrothermailerService brothermailerService)
        {
            _brothermailerService = brothermailerService;
            _logger = loggerFactory.CreateLogger<BrothermailerController>();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendEmail([FromBody]BrothermailerRequest brothermailerRequest)
        {   
            try
            {
                _logger.LogEnter();

                _logger.LogInformation("Signing up email address to brothermailer");
                
                var result = await _brothermailerService.SendEmailAddress(brothermailerRequest);
                
                return result.Accept(new BrothermailerResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}