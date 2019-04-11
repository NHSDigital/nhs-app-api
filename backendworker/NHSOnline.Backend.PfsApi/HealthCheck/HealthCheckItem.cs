namespace NHSOnline.Backend.PfsApi.HealthCheck
{
    public class HealthCheckItem
    {
        public string HealthCheckName { get; set; }
        public bool IsHealthy { get; set; }
        public string Message { get; set; }
    }
}