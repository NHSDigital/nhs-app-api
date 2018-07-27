namespace NHSOnline.Backend.Worker.Areas.HealthCheck.Models
{
    public class HealthCheckItem
    {
        public string HealthCheckName { get; set; }
        public bool IsHealthy { get; set; }
        public string Message { get; set; }
    }
}