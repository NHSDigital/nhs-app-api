using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public interface IVisionLinkageConfig
    {
        Uri ApiUrl { get; }
    }
}
