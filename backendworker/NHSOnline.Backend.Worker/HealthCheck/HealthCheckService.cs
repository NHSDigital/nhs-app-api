using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.HealthCheck.Models;
using NHSOnline.Backend.Worker.HealthCheck.Redis;

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
            var sessionCheckTask = _redisHealthCheckFactory.Create(ConnectionMultiplexerName.Session).Execute();

            await Task.WhenAll(odsCheckTask, sessionCheckTask);

            var odsCheck = await odsCheckTask;
            var sessionCheck = await sessionCheckTask;
            
            return new HealthCheckResponse
            {
                HealthChecks = new List<HealthCheckItem>
                {
                    new HealthCheckItem
                    {
                        IsHealthy = odsCheck.IsHealthy,
                        HealthCheckName = odsCheck.HealthCheckName,
                        Message = odsCheck.Message
                    },
                    new HealthCheckItem
                    {
                        IsHealthy = sessionCheck.IsHealthy,
                        HealthCheckName = sessionCheck.HealthCheckName,
                        Message = sessionCheck.Message
                    }
                }
            };
        }
    }
}