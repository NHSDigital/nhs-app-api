using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.HealthCheck.Models
{
    public class HealthCheckResponse
    {
        public HealthCheckResponse()
        {
            HealthChecks = new List<HealthCheckItem>();
        }    
        public List<HealthCheckItem> HealthChecks { get; set; }
    }
}