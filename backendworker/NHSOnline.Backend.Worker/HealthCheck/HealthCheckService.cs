using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.HealthCheck.Models;

namespace NHSOnline.Backend.Worker.HealthCheck
{
    public class HealthCheckService: IHealthCheckService
    {
        private readonly IRedisHealthCheckFactory _redisHealthCheckFactory;
        
        public HealthCheckService(IRedisHealthCheckFactory redisHealthCheckFactory)
        {
            _redisHealthCheckFactory = redisHealthCheckFactory;
        }

        public async Task<HealthCheckResponse> RunHealthChecks()
        {
            var odsCheckTask = _redisHealthCheckFactory.Create(ConnectionMultiplexerName.OdsCodeLookup).Execute();  

            await Task.WhenAll(odsCheckTask);

            var odsCheck = await odsCheckTask;
            
            return new HealthCheckResponse
            {
                HealthChecks = new List<HealthCheckItem>
                {
                    new HealthCheckItem
                    {
                        IsHealthy = odsCheck.IsHealthy,
                        HealthCheckName = odsCheck.HealthCheckName,
                        Message = odsCheck.Message
                    }
                }
            };
        }
    }
}