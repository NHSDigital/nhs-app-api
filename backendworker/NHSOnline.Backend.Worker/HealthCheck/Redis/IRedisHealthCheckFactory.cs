namespace NHSOnline.Backend.Worker.HealthCheck.Redis
{
    public interface IRedisHealthCheckFactory
    {
        IHealthCheck Create(ConnectionMultiplexerName multiplexerName);
    }
}