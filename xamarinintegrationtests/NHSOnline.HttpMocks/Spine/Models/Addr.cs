using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class Addr
    {
        [XmlElement(ElementName="streetAddressLine", Namespace="urn:hl7-org:v3", IsNullable = true)]
        [SuppressMessage("Usage", "CA1002:Collection properties should be read only",
            Justification = "Required for mocks")]
        public List<string?>? StreetAddressLine { get; set; }

        [XmlElement(ElementName="postalCode", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public string? PostalCode { get; set; }

        [XmlElement(ElementName="useablePeriod", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public UseablePeriod? UseablePeriod { get; set; }

        [XmlElement(ElementName="id", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Id? Id{ get; set; }

        [XmlAttribute(AttributeName="use")]
        public string? Use { get; set; }
    }
}