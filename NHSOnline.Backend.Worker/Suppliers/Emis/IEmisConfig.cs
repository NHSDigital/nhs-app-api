using System;

namespace NHSOnline.Backend.Worker.Suppliers.Emis
{
    public interface IEmisConfig
    {
        Uri BaseUrl { get; set; }
        string ApplicationId { get; set; }
        string Version { get; set; }
    }
}