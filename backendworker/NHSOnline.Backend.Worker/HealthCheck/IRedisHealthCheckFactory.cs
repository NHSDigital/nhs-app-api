namespace NHSOnline.Backend.Worker.HealthCheck
{
    public interface IRedisHealthCheckFactory
    {
        IHealthCheck Create(ConnectionMultiplexerName multiplexerName);
    }
}