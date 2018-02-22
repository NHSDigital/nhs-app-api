using Microsoft.Extensions.Configuration;

namespace NHSOnline.Backend.Worker.Interfaces
{
    public interface IConfig
    {
        string StubsEndpointUrl { get; set; }
    }
}
