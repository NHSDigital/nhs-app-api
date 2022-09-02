using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class Name
    {
        [XmlElement(ElementName="prefix", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public string? Prefix { get; set; }

        [SuppressMessage("Usage", "CA1002:Collection properties should be read only",
            Justification = "Required for mocks")]
        [XmlElement(ElementName="given", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public List<string?>? Given { get; set; }

        [XmlElement(ElementName="family", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public string? Family { get; set; }

        [XmlElement(ElementName="validTime", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public ValidTime? ValidTime { get; set; }

        [XmlElement(ElementName="id", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Id? Id { get; set; }

        [XmlAttribute(AttributeName="use")]
        public string? Use { get; set; }
    }
}