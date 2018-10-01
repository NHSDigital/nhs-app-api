using System;
using NHSOnline.Backend.Worker.Support.Certificate;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public interface ITppConfig: ICertificateConfig
    {
        Uri ApiUrl { get; set; }
        string ApiVersion { get; set; }
        string ApplicationName { get; set; }
        string ApplicationVersion { get; set; }
        string ApplicationProviderId { get; set; }
        string ApplicationDeviceType { get; set; }

        Guid CreateGuid();
    }
}