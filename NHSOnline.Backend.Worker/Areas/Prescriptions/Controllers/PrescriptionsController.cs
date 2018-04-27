using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions.Controllers
{
    [Route("patient/prescriptions")]
    public class PrescriptionsController : Controller
    {
        private readonly ConfigurationSettings _settings;
        private readonly ILogger<PrescriptionsController> _logger;

        public PrescriptionsController(IOptions<ConfigurationSettings> settings, ILoggerFactory loggerFactory)
        {
            _settings = settings.Value;
            _logger = loggerFactory.CreateLogger<PrescriptionsController>();
        }

        // TODO Insert session ID check
        [HttpGet]
        public IActionResult Get([FromQuery]DateTimeOffset? fromDate)
        {
            // If fromDate is null, we need to default to a date 6 months ago
            fromDate = fromDate ?? GetDefaultFromDate(); 

            var response = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>(),
                Courses = new List<Course>()
            };

            return Ok(response);
        }

        private DateTimeOffset GetDefaultFromDate()
        {
            return DateTimeOffset.Now.AddMonths(-_settings.PrescriptionsDefaultLastNumberMonthsToDisplay.Value);
        }
    }
}
