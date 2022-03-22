using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class Body
    {
        [XmlElement(ElementName="retrievalQueryResponse", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public RetrievalQueryResponse? RetrievalQueryResponse { get; set; }
    }
}