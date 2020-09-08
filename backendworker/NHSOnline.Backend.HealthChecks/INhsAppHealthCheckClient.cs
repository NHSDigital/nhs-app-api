using System.Net.Http;

namespace NHSOnline.Backend.HealthChecks
{
    public interface INhsAppHealthCheckClient
    {
        HttpClient Client { get; }
    }
}