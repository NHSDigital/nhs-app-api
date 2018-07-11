using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    [Route("patient/my-record")]
    public class MyRecordController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger _logger;
        
        public MyRecordController(
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = loggerFactory.CreateLogger<MyRecordController>();
        }

        [HttpGet]
        [TimeoutExceptionFilter]
        public async Task<IActionResult> GetMyRecord()
        {   
            var userSession = HttpContext.GetUserSession();
            
            var patientRecordService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetPatientRecordService();

            var result = await patientRecordService.Get(userSession);
            
            return result.Accept(new MyRecordResultVisitor());
        }
    }
}