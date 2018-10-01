using System;
using NHSOnline.Backend.Worker.Support.Certificate;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public interface IVisionConfig : ICertificateConfig
    {
        string ApplicationProviderId { get; }
        Uri ApiUrl { get; }
        string RequestUsername { get; }
    }
}
