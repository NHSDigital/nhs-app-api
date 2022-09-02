using System;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public abstract class AbstractTppRequestModel
    {
        [XmlAttribute("apiVersion")] public string? ApiVersion { get; set; }

        [XmlAttribute("uuid")] public Guid Uuid { get; set; }

        [XmlAttribute("unitId")] public string? UnitId { get; set; }
    }
}