using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.HealthCheck
{
    public abstract class HealthCheck : IHealthCheck
    {
        protected abstract string HealthCheckName { get; } 
        private readonly ILogger _logger;
        
        protected HealthCheck(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HealthCheck>();
        }

        public class Result
        {
            public bool IsHealthy { get; }
            public string HealthCheckName { get; }
            public string Message { get; }
            public Exception Error { get; }
               
            private Result(bool isHealthy, string healthCheckname, string message, Exception error)
            {
                IsHealthy = isHealthy;
                HealthCheckName = healthCheckname;
                Message = message;
                Error = error;
            }
            
            public static Result Healthy(string healthCheckName)
            {
                return new Result(true, healthCheckName, null, null);
            }
    
            public static Result Healthy(string healthCheckName, string message)
            {
                return new Result(true, healthCheckName, message, null);
            }
    
            public static Result Healthy(string healthCheckName, string message, object args)
            {
                return Healthy(healthCheckName, string.Format(message, args));
            }
    
            public static Result UnHealthy(string healthCheckName, string message)
            {
                return new Result(false, healthCheckName, message, null);
            }
    
            public static Result UnHealthy(string healthCheckName, string message, object args)
            {
                return UnHealthy(healthCheckName, string.Format(message, args));
            }
    
            public static Result UnHealthy(string healthCheckName, Exception error)
            {
                return new Result(false, healthCheckName, error.Message, error);
            }
        }
        
        protected abstract Task<Result> Check();

        public async Task<Result> Execute() 
        {
            try 
            {
                return await Check();
            } 
            catch (Exception e) 
            {
                _logger.LogError(e.Message);
                return Result.UnHealthy(HealthCheckName, e);
            }
        }
    }
}
