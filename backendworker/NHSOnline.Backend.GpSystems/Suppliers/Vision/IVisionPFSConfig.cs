using System;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public interface IVisionPFSConfig : ICertificateConfig
    {
        string ApplicationProviderId { get; }
        Uri ApiUrl { get; }
        string RequestUsername { get; }
        string VisionSenderUserName { get; }
        string VisionSenderUserFullName { get; }
        string VisionSenderUserIdentity { get; }
        string VisionSenderUserRole { get; }
    }
}
