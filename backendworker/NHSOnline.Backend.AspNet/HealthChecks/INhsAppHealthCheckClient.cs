using System.Net.Http;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    public interface INhsAppHealthCheckClient
    {
        HttpClient Client { get; }
    }
}