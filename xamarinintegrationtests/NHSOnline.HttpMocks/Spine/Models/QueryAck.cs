using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class QueryAck
    {
        [XmlElement(ElementName="queryResponseCode", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public QueryResponseCode? QueryResponseCode { get; set; }

        [XmlAttribute(AttributeName="type")]
        public string? Type { get; set; }
    }
}