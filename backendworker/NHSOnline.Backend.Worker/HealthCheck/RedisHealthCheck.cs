using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.HealthCheck
{
    public class RedisHealthCheck: BaseHealthCheck
    {
        protected override string HealthCheckName => string.Format(CultureInfo.InvariantCulture, "Redis {0}", _multiplexerName.ToString());
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private readonly ConnectionMultiplexerName _multiplexerName;
        
        public RedisHealthCheck(IConnectionMultiplexerFactory connectionMultiplexerFactory,
            ILoggerFactory loggerFactory,  ConnectionMultiplexerName multiplexerName) : base(loggerFactory)
        {
            _connectionMultiplexerFactory = connectionMultiplexerFactory;
            _multiplexerName = multiplexerName;
        }
        
        protected override async Task<Result> Check()
        {
            await _connectionMultiplexerFactory.GetMultiplexer(_multiplexerName).GetDatabase().PingAsync();
            return Result.Healthy(HealthCheckName);       
        }
    }
}