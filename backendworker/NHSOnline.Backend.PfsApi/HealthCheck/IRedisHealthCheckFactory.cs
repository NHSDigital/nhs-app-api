using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.HealthCheck
{
    public interface IRedisHealthCheckFactory
    {
        IHealthCheck Create(ConnectionMultiplexerName multiplexerName);
    }
}