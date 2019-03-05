
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.HealthCheck
{
    public class RedisHealthCheckFactory: IRedisHealthCheckFactory
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private readonly ILoggerFactory _loggerFactory;
        
        public RedisHealthCheckFactory(IConnectionMultiplexerFactory connectionMultiplexerFactory, ILoggerFactory loggerFactory)
        {
            _connectionMultiplexerFactory = connectionMultiplexerFactory;
            _loggerFactory = loggerFactory;
        }
        
        public IHealthCheck Create(ConnectionMultiplexerName multiplexerName)
        {
            return new RedisHealthCheck(_connectionMultiplexerFactory, _loggerFactory, multiplexerName);    
        }
    }
}