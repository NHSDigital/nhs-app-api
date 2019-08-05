using NHSOnline.Backend.GpSystems;

namespace NHSOnline.Backend.PfsApi.HealthCheck
{
    public interface IRedisHealthCheckFactory
    {
        IHealthCheck Create(ConnectionMultiplexerName multiplexerName);
    }
}