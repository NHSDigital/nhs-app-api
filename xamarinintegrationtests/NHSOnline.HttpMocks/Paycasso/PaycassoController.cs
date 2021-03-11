using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace NHSOnline.HttpMocks.Paycasso
{
    [Host("paycasso.stubs.local.bitraft.io")]
    public class PaycassoController : Controller
    {
        private readonly ILogger _logger;

        public PaycassoController(ILogger<PaycassoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/api/{*path}")]
        public IActionResult Get(string path)
        {
            _logger.LogInformation("Paycasso request: {Path}", path);

            foreach (var (key, value) in Request.Query)
            {
                _logger.LogInformation("{Key}={Value}", key, string.Join(";", value));
            }
            
            return new ObjectResult(new{});
        }

        [HttpPost("/api/transactions")]
        public async Task<IActionResult> PostTransactions()
        {
            await LogRequest();

            return new ObjectResult(new
            {
                transactionId = Guid.NewGuid().ToString()
            });
        }

        [HttpPost("/api/transactions/{id}/cancel")]
        public async Task<IActionResult> PostTransactionsCancel()
        {
            await LogRequest();

            return new OkResult();
        }

        private async Task LogRequest()
        {
            foreach (var (key, value) in Request.Query)
            {
                _logger.LogInformation("Paycasso request Query: {Key}={Value}", key, string.Join(";", value));
            }

            if (Request.ContentLength > 0)
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                _logger.LogInformation("Paycasso request Body: {Body}", body);
            }
        }

    }
}