using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    public interface ITppRequest
    {
        string RequestType { get; }

        string ApiVersion { get; set; }

        Guid Uuid { get; set; }
    }
}